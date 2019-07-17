using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ObjectPickerAttribute))]
	public class ObjectPickerDrawer : PropertyDrawer
	{
		private const string _invalidPropertyTypeWarning = "(PUOPDIPT) invalid type for ObjectPickerAttribute on field {0}: ObjectPicker can only be applied to Object or derived fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var baseType = (attribute as ObjectPickerAttribute)?.BaseType ?? this.GetFieldType();

			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				var field = new ObjectPickerField(property.displayName, property.objectReferenceValue, baseType);
				return field.ConfigureProperty(property);
			}
			else
			{
				Debug.LogWarningFormat(_invalidPropertyTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}
