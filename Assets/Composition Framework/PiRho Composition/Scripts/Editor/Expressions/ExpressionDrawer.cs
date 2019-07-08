using PiRhoSoft.Composition.Engine;
using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(Expression))]
	public class ExpressionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var expression = PropertyHelper.GetObject<Expression>(property);
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var textField = new TextField() { multiline = true };
			var messageBox = new MessageBox(MessageBoxType.Error, string.Empty);

			ElementHelper.SetVisible(messageBox, expression.HasError);
			ElementHelper.Bind(textField, textField, property.serializedObject.context, () => expression.Statement,
			value =>
			{
				expression.SetStatement(value);
				ElementHelper.SetVisible(messageBox, expression.HasError);
			});

			container.Add(textField);
			container.Add(messageBox);

			return container;
		}
	}
}
