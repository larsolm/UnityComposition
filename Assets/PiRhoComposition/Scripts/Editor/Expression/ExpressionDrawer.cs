using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(Expression))]
	public class ExpressionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var autocomplete = Autocomplete.GetItem(property.serializedObject.targetObject);
			var field = new ExpressionField(property.displayName, property.GetObject<Expression>(), autocomplete);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
