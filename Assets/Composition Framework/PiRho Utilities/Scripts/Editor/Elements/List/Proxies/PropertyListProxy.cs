using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyListProxy : ListProxy
	{
		public Action AddCallback;
		public Action<int> RemoveCallback;
		public Action<int, int> ReorderCallback;

		private SerializedProperty _property;
		private PropertyDrawer _drawer;

		public override int ItemCount => _property.arraySize;

		public PropertyListProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public override VisualElement CreateElement(int index)
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

		public override void AddItem()
		{
			_property.ResizeArray(_property.arraySize + 1);
			AddCallback?.Invoke();
		}

		public override void RemoveItem(int index)
		{
			RemoveCallback?.Invoke(index);
			_property.RemoveFromArray(index);
		}

		public override void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
			ReorderCallback?.Invoke(from, to);
		}
	}
}