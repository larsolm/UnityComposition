using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableDefinition))]
	public class VariableDefinitionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var nameProperty = property.FindPropertyRelative(nameof(VariableDefinition.Name));
			var definitionProperty = property.FindPropertyRelative(nameof(VariableDefinition.Definition));
			var container = ElementHelper.CreatePropertyContainer(nameProperty.stringValue);

			container.Add(new ValueDefinitionElement(definitionProperty));

			return container;
		}
	}
}
