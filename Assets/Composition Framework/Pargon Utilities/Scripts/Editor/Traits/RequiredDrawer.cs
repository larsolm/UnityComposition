using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	class RequiredDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITRDIT) Invalid type for RequiredAttribute on field {0}: Required can only be applied to string or Object fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.ObjectReference)
			{
				var required = attribute as RequiredAttribute;
				var column = new VisualElement();
				var message = new MessageBox();
				message.Setup(required.Type, required.Message);

				column.Add(container);
				column.Add(message);
				column.schedule.Execute(() =>
				{
					var visible = GetVisible(property);
					ElementHelper.SetVisible(message, visible);
				}).Every(100);

				return column;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}

		private bool GetVisible(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
				return string.IsNullOrEmpty(property.stringValue);
			else if (property.propertyType == SerializedPropertyType.ObjectReference)
				return property.objectReferenceValue == null;
			else
				return false;
		}
	}
}
