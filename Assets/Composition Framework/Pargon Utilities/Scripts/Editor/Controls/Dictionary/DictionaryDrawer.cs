using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(DictionaryAttribute))]
	class DictionaryDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUCDDIT) Invalid type for DictionaryAttribute on field '{0}': List can only be applied to SerializedDictionary fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Generic) // This might be wrong. Remember to check.
			{
				var itemDrawer = PropertyHelper.GetNextDrawer(fieldInfo, attribute);
				var proxy = itemDrawer != null ? new PropertyDrawerDictionaryProxy(property, itemDrawer) : new PropertyDictionaryProxy(property);
				return new DictionaryElement(proxy, property.displayName, ElementHelper.GetTooltip(fieldInfo));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new VisualElement();
			}
		}

		private class PropertyDrawerDictionaryProxy : PropertyDictionaryProxy
		{
			private PropertyDrawer _drawer;

			public PropertyDrawerDictionaryProxy(SerializedProperty property, PropertyDrawer drawer) : base(property)
			{
				_drawer = drawer;
			}

			public override VisualElement CreateValueElement(int index)
			{
				return _drawer.CreatePropertyGUI(ValuesProperty.GetArrayElementAtIndex(index));
			}
		}
	}
}
