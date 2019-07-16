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
			var container = new FieldContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var expression = PropertyHelper.GetObject<Expression>(property);
			var element = new ExpressionElement(property.serializedObject.targetObject, expression);

			container.Add(element);

			return container;
		}
	}
}
