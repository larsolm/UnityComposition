using PiRhoSoft.Utilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	class EnumButtonsDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUCEBDIT) Invalid type for EnumButtonsAttribute on field '{0}': EnumButtons can only be applied to Enum fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));

			if (property.propertyType == SerializedPropertyType.Enum)
			{
				var type = fieldInfo.FieldType;
				var flags = attribute is EnumButtonsAttribute enumButtons && enumButtons.Flags;
				var buttons = new EnumButtons(property);

				buttons.Setup(type, flags, buttons.GetEnumFromInt(type, property.intValue));
				container.Add(buttons);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
