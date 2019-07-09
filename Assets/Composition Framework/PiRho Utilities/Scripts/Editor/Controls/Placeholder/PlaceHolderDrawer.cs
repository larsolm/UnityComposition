using PiRhoSoft.Utilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(PlaceholderAttribute))]
	class PlaceholderDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PICPDIT) Invalid type for PlaceholderAttribute on field {0}: Placeholder can only be applied to string, int, or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var place = attribute as PlaceholderAttribute;

			if (property.propertyType == SerializedPropertyType.String)
				container.Add(new Placeholder<string>(place.Placeholder, property));
			else if (property.propertyType == SerializedPropertyType.Integer)
				container.Add(new Placeholder<int>(place.Placeholder, property));
			else if (property.propertyType == SerializedPropertyType.Float)
				container.Add(new Placeholder<float>(place.Placeholder, property));
			else
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);

			return container;
		}
	}
}
