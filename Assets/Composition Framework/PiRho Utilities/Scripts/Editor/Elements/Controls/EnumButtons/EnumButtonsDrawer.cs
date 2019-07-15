using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	class EnumButtonsDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUEEBDIT) invalid type for EnumButtonsAttribute on field '{0}': EnumButtons can only be applied to Enum fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				var type = PropertyHelper.GetFieldType(fieldInfo);
				var flags = attribute is EnumButtonsAttribute enumButtons ? enumButtons.Flags : null;
				var field = new EnumButtonsField(property, property.displayName, type, flags);

				return ElementHelper.SetupPropertyField(field, fieldInfo);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return ElementHelper.CreateEmptyPropertyField(property.displayName);
			}
		}
	}
}