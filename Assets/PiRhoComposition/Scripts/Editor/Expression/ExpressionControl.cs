using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ExpressionControl : VisualElement, IAutocompleteProxy
	{
		public const string Stylesheet = "Expression/ExpressionStyle.uss";
		public const string UssClassName = "pirho-expression";
		public const string InvalidUssClassName = UssClassName + "--invalid";
		public const string TextUssClassName = UssClassName + "__text";
		public const string MessageUssClassName = UssClassName + "__message";

		private readonly char[] _invalidCharacters = new char[] { '\n', '\t' };

		public Expression Value { get; private set; }

		private AutocompleteControl _autocompleteControl;
		private int _currentOpenIndex = 0;
		private int _currentCloseIndex = -1;

		private TextElement _measure;
		private TextField _textField;
		private MessageBox _message;
		private ExpressionCompilationResult _result;

		public ExpressionControl(Expression value, IAutocompleteItem autocomplete)
		{
			Value = value;
			Autocomplete = autocomplete;

			_textField = new TextField() { multiline = true };
			_textField.AddToClassList(TextUssClassName);
			_textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.Q(className: TextField.inputUssClassName).RegisterCallback<MouseUpEvent>(evt => RefreshAutocomplete());

			_autocompleteControl = new AutocompleteControl(this, _textField);
			_measure = new TextElement();

			_message = new MessageBox(MessageBoxType.Error, string.Empty);
			_message.AddToClassList(MessageUssClassName);
			_result = new ExpressionCompilationResult();

			Add(_textField);
			Add(_autocompleteControl);
			Add(_message);
			Add(_measure);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);

			Refresh();
		}

		public void SetValueWithoutNotify(string expression)
		{
			_result = Value.SetStatement(expression);
			Refresh();
		}

		private void Refresh()
		{
			EnableInClassList(InvalidUssClassName, !_result.Success);

			_message.Message = _result.Message;

			_textField.SetValueWithoutNotify(Value.Statement);
			RefreshAutocomplete();

			if (Variable.IndexOfAny(_invalidCharacters) >= 0)
				Variable = Variable.Replace("\t", "");
		}

		private void RefreshAutocomplete()
		{
			if (string.IsNullOrEmpty(_textField.text))
			{
				_currentOpenIndex = 0;
				_currentCloseIndex = -1;
			}
			else if (_textField.cursorIndex == 0)
			{
				_currentOpenIndex = 0;
				_currentCloseIndex = _textField.text.IndexOf(' ');
			}
			else
			{
				_currentOpenIndex = _textField.text.LastIndexOf(' ', _textField.cursorIndex - 1) + 1;
				_currentCloseIndex = _textField.text.IndexOf(' ', _currentOpenIndex);
			}

			_autocompleteControl.Refresh();
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			// Schedule these so cursorIndex is correct
			if (evt.keyCode == KeyCode.LeftArrow)
				schedule.Execute(RefreshAutocomplete).StartingIn(0);
			else if (evt.keyCode == KeyCode.RightArrow)
				schedule.Execute(RefreshAutocomplete).StartingIn(0);

			if (Autocomplete == null)
			{
				if (evt.keyCode == KeyCode.UpArrow)
					schedule.Execute(RefreshAutocomplete).StartingIn(0);
				else if (evt.keyCode == KeyCode.LeftArrow)
					schedule.Execute(RefreshAutocomplete).StartingIn(0);
			}
		}

		#region IAutocompleteItem Implementation

		public IAutocompleteItem Autocomplete { get; private set; }

		public Vector2 ControlPosition
		{
			get
			{
				var text = _currentOpenIndex <= 0 ? " " : _textField.text.Substring(0, _currentOpenIndex);
				var size = _measure.MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

				return _textField.worldBound.position + size;
			}
		}

		public string Variable
		{
			get
			{
				if (_currentCloseIndex < 0)
					return _textField.text.Substring(_currentOpenIndex);

				return _textField.text.Substring(_currentOpenIndex, _currentCloseIndex - _currentOpenIndex);
			}
			set
			{
				var previous = _textField.text;
				var prefix = _textField.text;

				if (_currentOpenIndex < prefix.Length)
				{
					if (_currentCloseIndex < 0)
						prefix = prefix.Remove(_currentOpenIndex);
					else
						prefix.Remove(_currentOpenIndex, _currentCloseIndex - _currentOpenIndex);
				}

				var next = prefix.Insert(_currentOpenIndex, value);

				_textField.value = next;
			}
		}

		public int Cursor
		{
			get
			{
				return _textField.cursorIndex - _currentOpenIndex;
			}
			set
			{
				var index = value + _currentOpenIndex + 1;
				_textField.SelectRange(index, index);
			}
		}

		#endregion
	}
}
