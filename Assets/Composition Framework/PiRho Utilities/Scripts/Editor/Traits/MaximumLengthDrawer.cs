using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumLengthAttribute))]
	class MaximumLengthDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITMLDIT) Invalid type for MaximumLengthAttribute on field {0}: MaximumLength can only be applied to string fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximum = attribute as MaximumLengthAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			if (property.propertyType == SerializedPropertyType.String)
			{
				var input = container.Query<TextField>().First();
				input.maxLength = maximum.Length;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
