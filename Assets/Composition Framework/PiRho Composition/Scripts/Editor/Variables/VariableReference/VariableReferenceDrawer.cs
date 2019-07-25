using PiRhoSoft.Utilities;
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
			var source = property.serializedObject.targetObject as IAutocompleteSource;
			var field = new VariableReferenceField(property.displayName, property.GetObject<VariableReference>(), source.AutocompleteSource);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
