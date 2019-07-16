using PiRhoSoft.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumAttribute))]
	class MaximumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITMADIT) Invalid type for MaximumAttribute on field {0}: Maximum can only be applied to int or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximum = attribute as MaximumAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				Clamp(property, Mathf.RoundToInt(maximum.Maximum));
				container.RegisterCallback<FocusOutEvent>(e => Clamp(property, Mathf.RoundToInt(maximum.Maximum)));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				Clamp(property, maximum.Maximum);
				container.RegisterCallback<FocusOutEvent>(e => Clamp(property, maximum.Maximum));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}

		private void Clamp(SerializedProperty property, int maximum)
		{
			property.intValue = Mathf.Min(maximum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Clamp(SerializedProperty property, float maximum)
		{
			property.floatValue = Mathf.Min(maximum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}
