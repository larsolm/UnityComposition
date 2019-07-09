using PiRhoSoft.Utilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DropdownAttribute))]
	class DropdownDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "Invalid type for DropdownAttribute on field {0}: Dropdown can only be applied to enum, string, int, or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var drop = attribute as DropdownAttribute;
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));

			if (property.propertyType == SerializedPropertyType.String)
				container.Add(new StringDropdown(drop.Options, drop.Options, property.stringValue, property));
			else if (property.propertyType == SerializedPropertyType.Integer)
				container.Add(new IntDropdown(drop.Options, drop.IntValues, property.intValue, property));
			else if (property.propertyType == SerializedPropertyType.Float)
				container.Add(new FloatDropdown(drop.Options, drop.FloatValues, property.floatValue, property));
			else if (property.propertyType == SerializedPropertyType.Enum)
				container.Add(new EnumDropdown(fieldInfo.FieldType, property.intValue, property));
			else
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);

			return container;
		}
	}
}
