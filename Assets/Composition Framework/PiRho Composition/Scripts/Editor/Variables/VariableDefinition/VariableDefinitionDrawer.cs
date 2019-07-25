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
			//var field = new VariableDefinitionField(property.displayName, property.GetObject<VariableDefinition>(), VariableInitializerType.None, null, false);
			//return field.ConfigureProperty(property, this.GetTooltip());
			return new VisualElement();
		}
	}
}
