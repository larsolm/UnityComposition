using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SchemaVariableCollection))]
	public class VariableSetDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new VariableSetField(property.displayName, property.GetObject<SchemaVariableCollection>(), property.serializedObject.targetObject);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
