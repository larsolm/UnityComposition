using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class PropertyListProxy : ListProxy
	{
		private SerializedProperty _property;
		private PropertyDrawer _drawer;

		public override int ItemCount => _property.arraySize;

		public PropertyListProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public override VisualElement CreateItem(int index)
		{
			var field = _drawer != null
				? _drawer.CreatePropertyGUI(_property.GetArrayElementAtIndex(index))
				: new ListPropertyField(_property.GetArrayElementAtIndex(index));

			field.userData = index;
			field.Bind(_property.serializedObject);

			return field;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem()
		{
			PropertyHelper.ResizeArray(_property, _property.arraySize + 1);
		}

		public override void RemoveItem(int index)
		{
			PropertyHelper.RemoveFromArray(_property, index);
		}

		public override void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}

		private class ListPropertyField : PropertyField
		{
			public ListPropertyField(SerializedProperty property) : base(property)
			{
			}

			protected override void ExecuteDefaultActionAtTarget(EventBase evt)
			{
				base.ExecuteDefaultActionAtTarget(evt);

				if (ElementHelper.IsSerializedPropertyBindEvent(evt, out var property))
				{
					// PropertyField forces the label of its base field to property.displayName during binding (specifically in
					// ConfigureField) when it is given a label of null or empty string. This will reset the label to null, which is
					// a valid state for BaseField.

					var baseField = this.Q(null, ElementHelper.BaseFieldUssClassName) as BindableElement;

					if (baseField != null)
						ElementHelper.SetBaseFieldLabel(baseField, null);
				}
			}
		}
	}
}