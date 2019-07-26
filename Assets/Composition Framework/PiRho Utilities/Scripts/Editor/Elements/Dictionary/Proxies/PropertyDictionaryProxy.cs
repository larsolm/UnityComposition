using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyDictionaryProxy : DictionaryProxy
	{
		public Action<string> AddCallback;
		public Action<string> RemoveCallback;
		public Action<string> ReorderCallback;

		private SerializedProperty _keysProperty;
		private SerializedProperty _valuesProperty;
		private PropertyDrawer _drawer;

		public override int KeyCount => _keysProperty.arraySize;

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
			for (var i = 0; i < KeyCount; i++)
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
			// TODO: Make sure ApplyModifyProperties doesn't trigger a serialization cycle and desync our SerializedDictionary

			_keysProperty.ResizeArray(KeyCount + 1);

			var newItem = _keysProperty.GetArrayElementAtIndex(_keysProperty.arraySize - 1);
			newItem.stringValue = key;

			_valuesProperty.ResizeArray(KeyCount);

			AddCallback?.Invoke(key);
		}

		public override void RemoveItem(int index)
		{
			var property = _keysProperty.GetArrayElementAtIndex(index);
			RemoveCallback?.Invoke(property.stringValue);

			_keysProperty.RemoveFromArray(index);
			_valuesProperty.RemoveFromArray(index);
		}

		public override void ReorderItem(int from, int to)
		{
			_keysProperty.MoveArrayElement(from, to);
			_valuesProperty.MoveArrayElement(from, to);

			_keysProperty.serializedObject.ApplyModifiedProperties();
			_valuesProperty.serializedObject.ApplyModifiedProperties();

			var property = _keysProperty.GetArrayElementAtIndex(to);
			ReorderCallback?.Invoke(property.stringValue);
		}
	}
}
