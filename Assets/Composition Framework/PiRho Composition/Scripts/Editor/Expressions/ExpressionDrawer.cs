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
			var field = new ExpressionField(property.displayName, property.GetObject<Expression>());
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
