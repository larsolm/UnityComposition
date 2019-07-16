using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ExpressionElement : VisualElement
	{
		public const string StyleSheetPath = Composition.StylePath + "Editor/Expressions/ExpressionElement.uss";
		public const string UssClassName = "pirho-expression";
		public const string UssTextClassName = UssClassName + "__text";
		public const string UssMessageClassName = UssClassName + "__message";

		public ExpressionElement(Object owner, Expression expression)
		{
			ElementHelper.AddStyleSheet(this, StyleSheetPath);
			AddToClassList(UssClassName);

			var textField = new TextField() { multiline = true };
			textField.AddToClassList(UssTextClassName);

			var messageBox = new MessageBox(MessageBoxType.Error, string.Empty);
			messageBox.AddToClassList(UssMessageClassName);
			
			ElementHelper.SetVisible(messageBox, expression.HasError);
			ElementHelper.Bind(textField, textField, owner, () => expression.Statement,
			value =>
			{
				expression.SetStatement(value);
				ElementHelper.SetVisible(messageBox, expression.HasError);
			});
			
			Add(textField);
			Add(messageBox);
		}
	}
}
