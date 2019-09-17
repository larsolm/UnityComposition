using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var item = Autocomplete.GetItem(property.serializedObject.targetObject);
			var field = new VariableReferenceField(property.displayName, property.GetObject<VariableReference>(), item);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
