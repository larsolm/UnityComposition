using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ListField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for ListField: property '{1}' is type '{2}' but should be an array";

		public static readonly string UssClassName = "pirho-list-field";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; }
		public string EmptyTooltip { get; set; }
		public string AddTooltip { get; set; }
		public string RemoveTooltip { get; set; }
		public string ReorderTooltip { get; set; }

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;
		public bool AllowReorder { get; set; } = true;

		public Func<bool> CanAdd;
		public Func<int, bool> CanRemove;
		public Func<int, int, bool> CanReorder;

		public Action AddCallback;
		public Action<int> RemoveCallback;
		public Action<int, int> ReorderCallback;

		private PropertyDrawer _drawer;
		private ListControl _control;
		private ChangeTriggerControl<int> _sizeBinding;

		public ListField(SerializedProperty property, PropertyDrawer drawer)
		{
			_drawer = drawer;
			bindingPath = property.propertyPath;
		}

		private void Setup(PropertyListProxy proxy)
		{
			Clear();

			if (Label != null) proxy.Label = Label;
			if (EmptyLabel != null) proxy.EmptyLabel = EmptyLabel;
			if (EmptyTooltip != null) proxy.EmptyTooltip = EmptyTooltip;
			if (AddTooltip != null) proxy.AddTooltip = AddTooltip;
			if (RemoveTooltip != null) proxy.RemoveTooltip = RemoveTooltip;
			if (ReorderTooltip != null) proxy.ReorderTooltip = ReorderTooltip;

			proxy.Tooltip = Tooltip;
			proxy.AllowAdd = AllowAdd;
			proxy.AllowRemove = AllowRemove;
			proxy.AllowReorder = AllowReorder;
			proxy.AddCallback = AddCallback;
			proxy.RemoveCallback = RemoveCallback;
			proxy.ReorderCallback = ReorderCallback;
			proxy.CanAddCallback = CanAdd;
			proxy.CanRemoveCallback = CanRemove;
			proxy.CanReorderCallback = CanReorder;

			_control = new ListControl(proxy);

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
					Setup(new PropertyListProxy(property, _drawer));

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