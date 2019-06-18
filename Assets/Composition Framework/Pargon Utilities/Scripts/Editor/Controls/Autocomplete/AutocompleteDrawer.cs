using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonInspector.Editor
{
	[CustomPropertyDrawer(typeof(AutocompleteAttribute))]
	class AutocompleteDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);
			var element = new AutocompleteElement();
			var type = (attribute as AutocompleteAttribute).SourceType;

			var source = Activator.CreateInstance(type) as AutocompleteSource;

			element.Setup(property, source);
			container.Add(element);

			return container;
		}
	}
}
