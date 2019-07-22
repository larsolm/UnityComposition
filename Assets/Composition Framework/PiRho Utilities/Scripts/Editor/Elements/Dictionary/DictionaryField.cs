using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for DictionaryField: property '{1}' is type '{2}' but should be an array";

		public static readonly string UssClassName = "pirho-list-field";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; }
		public string EmptyTooltip { get; set; }
		public string AddTooltip { get; set; } = DictionaryProxy.DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DictionaryProxy.DefaultRemoveTooltip;

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;

		private PropertyDrawer _drawer;
		private DictionaryControl _control;
		private ChangeTriggerControl<int> _sizeBinding;

		public DictionaryField(SerializedProperty keysProperty, PropertyDrawer drawer)
		{
			_drawer = drawer;
			bindingPath = keysProperty.propertyPath;
		}

		private void Setup(DictionaryProxy proxy)
		{
			Clear();

			proxy.Label = Label;
			proxy.Tooltip = Tooltip;

			if (EmptyLabel != null) proxy.EmptyLabel = EmptyLabel;
			if (EmptyTooltip != null) proxy.EmptyTooltip = EmptyTooltip;
			if (AddTooltip != null) proxy.AddTooltip = AddTooltip;
			if (RemoveTooltip != null) proxy.RemoveTooltip = RemoveTooltip;

			proxy.AllowAdd = AllowAdd;
			proxy.AllowRemove = AllowRemove;

			_control = new DictionaryControl(proxy);

			Add(_control);
			AddToClassList(UssClassName);
		}

		#region Binding

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				if (property.isArray)
				{
					Setup(new PropertyDictionaryProxy(property, _drawer));

					if (_sizeBinding == null)
						_sizeBinding = new ChangeTriggerControl<int>(null, (oldSize, size) => _control.Refresh());

					Add(_sizeBinding); // setup calls Clear so this always needs to be added
					_sizeBinding.Watch(property.FindPropertyRelative("Array.size"));
				}
				else
				{
					Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
				}
			}
		}

		#endregion

		#region UXML Support

		public DictionaryField() { }

		public new class UxmlFactory : UxmlFactory<DictionaryField, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var list = ve as DictionaryField;

				// TODO: attributes for proxy properties
				// TODO: if !bindingPath, call Setup with a proxy that owns a list holding objects of type specified as an attribute
			}
		}

		#endregion
	}
}