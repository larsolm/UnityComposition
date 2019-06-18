using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(PlaceholderAttribute))]
	class PlaceholderDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PICPDIT) Invalid type for PlaceholderAttribute on field {0}: Placeholder can only be applied to string, int, or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);
			var place = attribute as PlaceholderAttribute;

			if (property.propertyType == SerializedPropertyType.String)
			{
				var placeholder = new Placeholder<string>();
				placeholder.Setup(place.Placeholder, property);
				container.Add(placeholder);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				var placeholder = new Placeholder<int>();
				placeholder.Setup(place.Placeholder, property);
				container.Add(placeholder);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var placeholder = new Placeholder<float>();
				placeholder.Setup(place.Placeholder, property);
				container.Add(placeholder);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
