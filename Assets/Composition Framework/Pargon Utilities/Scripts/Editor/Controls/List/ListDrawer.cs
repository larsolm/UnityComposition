using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(ListAttribute))]
	class ListDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUCLDIT) Invalid type for ListAttribute on field '{0}': List can only be applied to SerializedList or Array fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Generic) // This might be wrong. Remember to check.
			{
				var items = property.FindPropertyRelative("_items");
				var itemDrawer = PropertyHelper.GetNextDrawer(fieldInfo, attribute);
				var proxy = itemDrawer != null ? new PropertyDrawerListProxy(items, itemDrawer) : new PropertyListProxy(items);

				return new ListElement(proxy, property.displayName, ElementHelper.GetTooltip(fieldInfo));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new VisualElement();
			}
		}

		private class PropertyDrawerListProxy : PropertyListProxy
		{
			private PropertyDrawer _drawer;

			public PropertyDrawerListProxy(SerializedProperty property, PropertyDrawer drawer) : base(property)
			{
				_drawer = drawer;
			}

			public override VisualElement CreateElement(int index)
			{
				return _drawer.CreatePropertyGUI(Property.GetArrayElementAtIndex(index));
			}
		}
	}
}
