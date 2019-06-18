using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	class EnumButtonsDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PICEBDIT) Invalid type for EnumButtonsAttribute on field {0}: EnumButtons can only be applied to Enum fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);

			if (property.propertyType == SerializedPropertyType.Enum)
			{
				var type = fieldInfo.FieldType;
				var flags = attribute is EnumButtonsAttribute enumButtons && enumButtons.Flags;
				var buttons = new EnumButtons();

				buttons.Setup(type, flags, property);
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
