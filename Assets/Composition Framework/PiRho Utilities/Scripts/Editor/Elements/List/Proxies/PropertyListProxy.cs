using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
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
			var property = _property.GetArrayElementAtIndex(index);

			var field = _drawer != null
				? _drawer.CreatePropertyGUI(property)
				: PropertyFieldExtensions.CreateFieldFromProperty(property);

			field.userData = index;
			field.Bind(_property.serializedObject);
			BaseFieldExtensions.SetLabel(field, null);

			return field;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem()
		{
			_property.ResizeArray(_property.arraySize + 1);
		}

		public override void RemoveItem(int index)
		{
			_property.RemoveFromArray(index);
		}

		public override void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}