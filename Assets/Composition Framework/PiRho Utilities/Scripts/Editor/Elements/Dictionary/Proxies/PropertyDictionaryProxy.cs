using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyDictionaryProxy : DictionaryProxy
	{
		private SerializedProperty _keysProperty;
		private SerializedProperty _valuesProperty;
		private PropertyDrawer _drawer;

		public override int ItemCount => _keysProperty.arraySize;

		public PropertyDictionaryProxy(SerializedProperty keys, SerializedProperty values, PropertyDrawer drawer)
		{
			_keysProperty = keys;
			_valuesProperty = values;
			_drawer = drawer;
		}

		public override VisualElement CreateField(int index)
		{
			var key = _keysProperty.GetArrayElementAtIndex(index);
			var value = _valuesProperty.GetArrayElementAtIndex(index);

			var field = _drawer != null
				? _drawer.CreatePropertyGUI(value)
				: value.CreateField();

			field.userData = index;
			field.Bind(_valuesProperty.serializedObject);
			BaseFieldExtensions.SetLabel(field, key.stringValue);

			return field;
		}

		public override bool IsKeyValid(string key)
		{
			for (var i = 0; i < ItemCount; i++)
			{
				var name = _keysProperty.GetArrayElementAtIndex(i);
				if (name.stringValue == key)
					return false;
			}

			return true;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem(string key)
		{
			_keysProperty.arraySize++;

			var newItem = _keysProperty.GetArrayElementAtIndex(_keysProperty.arraySize - 1);
			newItem.stringValue = key;

			_valuesProperty.ResizeArray(ItemCount);
		}

		public override void RemoveItem(int index)
		{
			_keysProperty.RemoveFromArray(index);
			_valuesProperty.RemoveFromArray(index);
		}
	}
}
