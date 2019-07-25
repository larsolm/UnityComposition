using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(ValueDefinition))]
	public class ValueDefinitionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			//var field = new ValueDefinitionField(property.displayName, property.GetObject<ValueDefinition>(), VariableInitializerType.None, null, false);
			//return field.ConfigureProperty(property, this.GetTooltip());
			return new VisualElement();
		}
	}
}
