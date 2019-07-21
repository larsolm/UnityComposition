using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyDictionaryProxy : DictionaryProxy
	{
		private SerializedProperty _property;
		private PropertyDrawer _drawer;

		public override int ItemCount => _property.arraySize;

		public PropertyDictionaryProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public override VisualElement CreateField(int index)
		{
			var property = _property.GetArrayElementAtIndex(index);

			var field = _drawer != null
				? _drawer.CreatePropertyGUI(property)
				: property.CreateField();

			field.userData = index;
			field.Bind(_property.serializedObject);
			BaseFieldExtensions.SetLabel(field, null);

			return field;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem(string key)
		{
			_property.ResizeArray(_property.arraySize + 1);
		}

		public override void RemoveItem(int index)
		{
			_property.RemoveFromArray(index);
		}
	}
}