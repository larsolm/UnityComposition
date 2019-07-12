using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ListField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for ListField: property '{1}' is type '{2}' but should be an array";

		public static readonly string UssClassName = "pirho-list-field";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; }
		public string EmptyTooltip { get; set; }
		public string AddTooltip { get; set; } = ListProxy.DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = ListProxy.DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = ListProxy.DefaultReorderTooltip;

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;
		public bool AllowReorder { get; set; } = true;

		private PropertyDrawer _drawer;
		private ListControl _control;
		private SimpleBinding<int> _sizeBinding;

		public ListField(SerializedProperty property, PropertyDrawer drawer)
		{
			_drawer = drawer;
			bindingPath = property.propertyPath;
		}

		private void Setup(ListProxy proxy)
		{
			Clear();

			proxy.Label = Label;
			proxy.Tooltip = Tooltip;

			if (EmptyLabel != null) proxy.EmptyLabel = EmptyLabel;
			if (EmptyTooltip != null) proxy.EmptyTooltip = EmptyTooltip;
			if (AddTooltip != null) proxy.AddTooltip = AddTooltip;
			if (RemoveTooltip != null) proxy.RemoveTooltip = RemoveTooltip;
			if (ReorderTooltip != null) proxy.ReorderTooltip = ReorderTooltip;

			proxy.AllowAdd = AllowAdd;
			proxy.AllowRemove = AllowRemove;
			proxy.AllowReorder = AllowReorder;

			_control = new ListControl(proxy);

			Add(_control);
			AddToClassList(UssClassName);
		}

		#region Array Resize

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (ElementHelper.IsSerializedPropertyBindEvent(evt, out var property))
			{
				if (property.isArray)
				{
					Setup(new PropertyListProxy(property, _drawer));

					if (_sizeBinding == null)
						_sizeBinding = new SimpleBinding<int>(property.arraySize, size => _control.Refresh());

					Add(_sizeBinding); // setup calls Clear so this always needs to be added
					_sizeBinding.Reset(property.FindPropertyRelative("Array.size"));
				}
				else
				{
					Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
				}
			}
		}

		#endregion

		#region UXML Support

		public ListField() { }

		public new class UxmlFactory : UxmlFactory<ListField, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var list = ve as ListField;

				// TODO: attributes for proxy properties
				// TODO: if !bindingPath, call Setup with a proxy that owns a list holding objects of type specified as an attribute
			}
		}

		#endregion
	}
}