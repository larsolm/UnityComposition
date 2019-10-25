using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableLookupReference), true)]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var autocomplete = Autocomplete.GetItem(property.serializedObject.targetObject);
			return new VariableReferenceField(property, autocomplete);
		}
	}
}
