using PiRhoSoft.Utilities;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class DictionaryProxy
	{
		public abstract int Count { get; }

		public abstract VisualElement CreateAddElement();
		public abstract VisualElement CreateKeyElement(int index);
		public abstract VisualElement CreateValueElement(int index);

		public abstract void AddItem();
		public abstract void RemoveItem(int index);
	}

	public class PropertyDictionaryProxy : DictionaryProxy
	{
		public SerializedProperty KeysProperty { get; private set; }
		public SerializedProperty ValuesProperty { get; private set; }

		private VisualElement _addElement;

		public override int Count => KeysProperty.arraySize;

		public PropertyDictionaryProxy(SerializedProperty property)
		{
			KeysProperty = property.FindPropertyRelative("_keys");
			ValuesProperty = property.FindPropertyRelative("_values");
		}

		public override VisualElement CreateAddElement()
		{
			AddItem(); // Add an element in case there are zero to make a default instance.

			_addElement = ElementHelper.CreateDefaultElement(KeysProperty.GetArrayElementAtIndex(KeysProperty.arraySize - 1));

			RemoveItem(KeysProperty.arraySize - 1);

			return _addElement;
		}

		public override VisualElement CreateKeyElement(int index)
		{
			var field = new PropertyField(KeysProperty.GetArrayElementAtIndex(index), null) { label = null };
			field.Bind(KeysProperty.serializedObject);
			return field;
		}

		public override VisualElement CreateValueElement(int index)
		{
			var field = new PropertyField(ValuesProperty.GetArrayElementAtIndex(index)) { label = null };
			field.Bind(ValuesProperty.serializedObject);
			return field;
		}

		public override void AddItem()
		{
			KeysProperty.arraySize++;
			ValuesProperty.arraySize++;

			var key = KeysProperty.GetArrayElementAtIndex(KeysProperty.arraySize - 1);
			ElementHelper.SetPropertyToElementValue(key, _addElement);

			// The newly added item will be a copy of the previous last item so reset it
			PropertyHelper.SetToDefault(ValuesProperty.GetArrayElementAtIndex(ValuesProperty.arraySize - 1));
			KeysProperty.serializedObject.ApplyModifiedProperties();
			ValuesProperty.serializedObject.ApplyModifiedProperties();
		}

		public override void RemoveItem(int index)
		{
			var key = KeysProperty.GetArrayElementAtIndex(index);
			var value = ValuesProperty.GetArrayElementAtIndex(index);

			// If an element is removed from a SerializedProperty that is a list or array of Objects,
			// DeleteArrayElementAtIndex will set the entry to null instead of removing it. If the entry is already
			// null it will be removed as expected.
			if (key.propertyType == SerializedPropertyType.ObjectReference)
				key.objectReferenceValue = null;

			if (value.propertyType == SerializedPropertyType.ObjectReference)
				key.objectReferenceValue = null;

			KeysProperty.DeleteArrayElementAtIndex(index);
			KeysProperty.serializedObject.ApplyModifiedProperties();

			ValuesProperty.DeleteArrayElementAtIndex(index);
			ValuesProperty.serializedObject.ApplyModifiedProperties();
		}
	}

	public class DictionaryProxy<KeyType, ValueType> : DictionaryProxy
	{
		private Func<KeyType> _addKey;
		private Func<VisualElement> _addElement;
		private Func<KeyType, VisualElement> _keyElement;
		private Func<ValueType, VisualElement> _valueElement;
		private IEditableDictionary<KeyType, ValueType> _dictionary;

		public override int Count => _dictionary.Count;

		public DictionaryProxy(IEditableDictionary<KeyType, ValueType> dictionary, Func<KeyType> addKey, Func<VisualElement> addElement, Func<KeyType, VisualElement> keyElement, Func<ValueType, VisualElement> valueElement)
		{
			_addKey = addKey;
			_addElement = addElement;
			_keyElement = keyElement;
			_valueElement = valueElement;
			_dictionary = dictionary;
			_dictionary.PrepareForEdit();
		}

		public override VisualElement CreateAddElement()
		{
			return _addElement();
		}

		public override VisualElement CreateKeyElement(int index)
		{
			return _keyElement(_dictionary.GetKey(index));
		}

		public override VisualElement CreateValueElement(int index)
		{
			return _valueElement(_dictionary.GetValue(index));
		}

		public override void AddItem()
		{
			var value = _addKey();
			_dictionary.Add(value, default);
			_dictionary.ApplyEdits();
		}

		public override void RemoveItem(int index)
		{
			var value = _dictionary.GetKey(index);
			_dictionary.Remove(value);
			_dictionary.ApplyEdits();
		}
	}
}