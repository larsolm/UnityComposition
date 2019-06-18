using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(DropdownAttribute))]
	class DropdownDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "Invalid type for DropdownAttribute on field {0}: Dropdown can only be applied to string, int, or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var drop = attribute as DropdownAttribute;
			var container = ElementHelper.CreatePropertyContainer(property.displayName);

			if (property.propertyType == SerializedPropertyType.String)
			{
				var dropdown = new StringDropdown();
				dropdown.Setup(drop.Options, drop.Options, property);

				container.Add(dropdown);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				var dropdown = new IntDropdown();
				dropdown.Setup(drop.Options, drop.IntValues, property);

				container.Add(dropdown);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var dropdown = new FloatDropdown();
				dropdown.Setup(drop.Options, drop.FloatValues, property);

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
