using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(ObjectPickerAttribute))]
	public class ObjectPickerDrawer : PropertyDrawer
	{
		private const string _invalidPropertyTypeWarning = "(PUCOPDIPT) Invalid type for ObjectPickerAttribute on field {0}: ObjectPicker can only be applied to Object or string fields";
		private const string _invalidBaseTypeWarning = "(PUCOPDIBT) Invalid base type for ObjectPickerAttribute on field {0}: the base type on ObjectPicker must be derived from Object";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var objectPicker = attribute as ObjectPickerAttribute;
			var type = objectPicker.BaseType ?? fieldInfo.FieldType;

			if (property.propertyType == SerializedPropertyType.ObjectReference || property.propertyType == SerializedPropertyType.String)
			{
				if (typeof(Object).IsAssignableFrom(type))
				{
					var picker = new ObjectPicker(property);

					if (property.propertyType == SerializedPropertyType.ObjectReference)
						picker.Setup(type, property.objectReferenceValue);
					else if (property.propertyType == SerializedPropertyType.String)
						picker.Setup(type, property.stringValue);

					container.Add(picker);
				}
				else
				{
					Debug.LogWarningFormat(_invalidBaseTypeWarning, property.propertyPath);
				}
			}
			else
			{
				Debug.LogWarningFormat(_invalidPropertyTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
