using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
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
			{
				var dropdown = new StringDropdown(property);
				dropdown.Setup(drop.Options, drop.Options, property.stringValue);

				container.Add(dropdown);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				var dropdown = new IntDropdown(property);
				dropdown.Setup(drop.Options, drop.IntValues, property.intValue);

				container.Add(dropdown);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var dropdown = new FloatDropdown(property);
				dropdown.Setup(drop.Options, drop.FloatValues, property.floatValue);

				container.Add(dropdown);
			}
			else if (property.propertyType == SerializedPropertyType.Enum)
			{
				var dropdown = new EnumDropdown(property);
				dropdown.Setup(fieldInfo.FieldType, property.intValue);

				container.Add(dropdown);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
