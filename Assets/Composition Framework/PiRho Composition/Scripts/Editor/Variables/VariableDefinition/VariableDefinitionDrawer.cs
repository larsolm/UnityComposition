using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
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
