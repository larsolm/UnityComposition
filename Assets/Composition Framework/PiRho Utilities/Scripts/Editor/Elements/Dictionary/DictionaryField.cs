﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for DictionaryField: property '{1}' is type '{2}' but should be an array";

		public static readonly string UssClassName = "pirho-dictionary-field";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; }
		public string EmptyTooltip { get; set; }
		public string AddPlaceholder { get; set; }
		public string AddTooltip { get; set; }
		public string RemoveTooltip { get; set; }
		public string ReorderTooltip { get; set; }

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;
		public bool AllowReorder { get; set; } = false;

		public Func<string, bool> CanAdd;
		public Func<string, bool> CanRemove;
		public Func<string, bool> CanReorder;

		public Action<string> AddCallback;
		public Action<string> RemoveCallback;
		public Action<string> ReorderCallback;

		private PropertyDictionaryProxy _proxy;
		private DictionaryControl _control;
		private ChangeTriggerControl<int> _sizeBinding;

		public DictionaryField(SerializedProperty property, SerializedProperty keysProperty, SerializedProperty valuesProperty, PropertyDrawer drawer)
		{
			_proxy = new PropertyDictionaryProxy(property, keysProperty, valuesProperty, drawer);
			bindingPath = keysProperty.propertyPath;
		}

		private void Setup()
		{
			Clear();

			if (EmptyLabel != null) _proxy.EmptyLabel = EmptyLabel;
			if (EmptyTooltip != null) _proxy.EmptyTooltip = EmptyTooltip;
			if (AddPlaceholder != null) _proxy.AddPlaceholder = AddPlaceholder;
			if (AddTooltip != null) _proxy.AddTooltip = AddTooltip;
			if (RemoveTooltip != null) _proxy.RemoveTooltip = RemoveTooltip;

			_proxy.Tooltip = Tooltip;
			_proxy.AllowAdd = AllowAdd;
			_proxy.AllowRemove = AllowRemove;
			_proxy.AllowReorder = AllowReorder;
			_proxy.AddCallback = AddCallback;
			_proxy.RemoveCallback = RemoveCallback;
			_proxy.ReorderCallback = ReorderCallback;
			_proxy.CanAddCallback = CanAdd;
			_proxy.CanRemoveCallback = CanRemove;
			_proxy.CanReorderCallback = CanReorder;

			_control = new DictionaryControl(_proxy);

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
					Setup();

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