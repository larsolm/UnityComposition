using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ExpressionControl : VisualElement
	{
		public Expression Value { get; private set; }

		private TextField _text;
		private MessageBox _message;

		public ExpressionControl(Expression value)
		{
			Value = value;

			_text = new TextField() { multiline = true };
			_text.RegisterValueChangedCallback(evt => Value.SetStatement(evt.newValue));

			_message = new MessageBox(MessageBoxType.Error, string.Empty);

			Add(_text);
			Add(_message);

			Refresh();
		}

		public void SetValueWithoutNotify(Expression expression)
		{
			Value = expression;
			Refresh();
		}

		private void Refresh()
		{
			_text.SetValueWithoutNotify(Value.Statement);
			_message.SetDisplayed(Value.HasError);
			_message.Message = Value.CompilationResult.Message;
		}
	}
}
