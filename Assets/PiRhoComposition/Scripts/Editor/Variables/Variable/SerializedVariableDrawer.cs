using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SerializedVariable))]
	public class SerializedVariableDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var constraint = fieldInfo.GetAttribute<VariableConstraintAttribute>();
			var definition = constraint?.GetDefinition(string.Empty);

			var field = new SerializedVariableField(property.displayName, definition);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
