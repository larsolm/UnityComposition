using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class ListProxy
	{
		public abstract int Count { get; }
		public abstract VisualElement CreateElement(int index);

		public abstract void AddItem();
		public abstract void RemoveItem(int index);
		public abstract void MoveItem(int from, int to);
	}

	public class PropertyListProxy : ListProxy
	{
		public SerializedProperty Property { get; private set; }

		public PropertyListProxy(SerializedProperty property)
		{
			Property = property;
		}

		public override int Count => Property.arraySize;

		public override VisualElement CreateElement(int index)
		{
			var field = new PropertyField(Property.GetArrayElementAtIndex(index));
			field.Bind(Property.serializedObject);
			field.label = " "; // no way to not show a label
			return field;
		}

		public override void AddItem()
		{
			Property.arraySize += 1;

			// The newly added item will be a copy of the previous last item so reset it
			PropertyHelper.SetToDefault(Property.GetArrayElementAtIndex(Property.arraySize - 1));
			Property.serializedObject.ApplyModifiedProperties();
		}

		public override void RemoveItem(int index)
		{
			var item = Property.GetArrayElementAtIndex(index);

			// If an element is removed from a SerializedProperty that is a list or array of Objects,
			// DeleteArrayElementAtIndex will set the entry to null instead of removing it. If the entry is already
			// null it will be removed as expected.
			if (item.propertyType == SerializedPropertyType.ObjectReference && item.objectReferenceValue != null)
				item.objectReferenceValue = null;

			Property.DeleteArrayElementAtIndex(index);
			Property.serializedObject.ApplyModifiedProperties();
		}

		public override void MoveItem(int from, int to)
		{
			Property.MoveArrayElement(from, to);
			Property.serializedObject.ApplyModifiedProperties();
		}
	}

	public class IListListProxy<T> : ListProxy
	{
		private IList<T> _list;
		private Func<T, VisualElement> _creator;

		public IListListProxy(IList<T> list, Func<T, VisualElement> creator)
		{
			_list = list;
			_creator = creator;
		}

		public override int Count { get; }

		public override VisualElement CreateElement(int index)
		{
			return _creator(_list[index]);
		}

		public override void AddItem()
		{
			_list.Add(default);
		}

		public override void RemoveItem(int index)
		{
			_list.RemoveAt(index);
		}

		public override void MoveItem(int from, int to)
		{
			var item = _list[from];
			_list[from] = _list[to];
			_list[to] = item;
		}
	}

	public class ArrayListProxy<T> : ListProxy
	{
		private T[] _array;
		private Func<T, VisualElement> _creator;

		public ArrayListProxy(T[] array, Func<T, VisualElement> creator)
		{
			_array = array;
			_creator = creator;
		}

		public override int Count => _array.Length;

		public override VisualElement CreateElement(int index)
		{
			return _creator(_array[index]);
		}

		public override void AddItem()
		{
		}

		public override void RemoveItem(int index)
		{
		}

		public override void MoveItem(int from, int to)
		{
			var item = _array[from];
			_array[from] = _array[to];
			_array[to] = item;
		}
	}
}