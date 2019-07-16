using PiRhoSoft.Utilities;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(AutocompleteAttribute))]
	class AutocompleteDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var element = new AutocompleteElement(property);
			var type = (attribute as AutocompleteAttribute).SourceType;

			var source = Activator.CreateInstance(type) as AutocompleteSource;

			element.Setup(source);
			container.Add(element);

			return container;
		}
	}
}
