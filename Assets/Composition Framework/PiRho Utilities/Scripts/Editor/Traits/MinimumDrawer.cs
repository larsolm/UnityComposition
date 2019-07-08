using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(MinimumAttribute))]
	class MinimumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITMIDIT) Invalid type for MinimumAttribute on field {0}: Minimum can only be applied to int or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var minimum = attribute as MinimumAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				Clamp(property, Mathf.RoundToInt(minimum.Minimum));
				container.RegisterCallback<FocusOutEvent>(e => Clamp(property, Mathf.RoundToInt(minimum.Minimum)));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				Clamp(property, minimum.Minimum);
				container.RegisterCallback<FocusOutEvent>(e => Clamp(property, minimum.Minimum));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}

		private void Clamp(SerializedProperty property, int minimum)
		{
			property.intValue = Mathf.Max(minimum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Clamp(SerializedProperty property, float minimum)
		{
			property.floatValue = Mathf.Max(minimum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}
