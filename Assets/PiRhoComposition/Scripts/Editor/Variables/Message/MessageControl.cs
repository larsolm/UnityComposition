using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class MessageControl : VisualElement, IAutocompleteProxy
	{
		public const string Stylesheet = "Variables/Message/MessageStyle.uss";
		public const string UssClassName = "pirho-message";
		public const string TextUssClassName = UssClassName + "__text";

		private readonly char[] _invalidCharacters = new char[] { '\n', '\t' };

		public Message Value { get; private set; }

		private AutocompleteControl _autocompleteControl;
		private readonly IAutocompleteItem _autocomplete;
		private int _currentOpenIndex = -1;
		private int _currentCloseIndex = -1;

		private TextElement _measure;
		private TextField _textField;

		public MessageControl(Message value, IAutocompleteItem autocomplete)
		{
			Value = value;

			_textField = new TextField { multiline = true };
			_textField.AddToClassList(TextUssClassName);
			_textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.Q(className: TextField.inputUssClassName).RegisterCallback<MouseUpEvent>(evt => RefreshAutocomplete());

			_autocomplete = autocomplete;
			_autocompleteControl = new AutocompleteControl(this, _textField);
			_measure = new TextElement();

			Add(_textField);
			Add(_autocompleteControl);
			Add(_measure);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);

			Refresh();
		}

		public void SetValueWithoutNotify(string newValue)
		{
			Value.Text = newValue;
			Refresh();
		}

		private void Refresh()
		{
			_textField.SetValueWithoutNotify(Value.Text);
			RefreshAutocomplete();

			var variable = Variable;
			if (variable.IndexOfAny(_invalidCharacters) >= 0)
			{
				variable = variable.Replace("\n", "");
				variable = variable.Replace("\t", "");

				Variable = variable;
			}
		}

		private void RefreshAutocomplete()
		{
			_currentOpenIndex = -1;
			_currentCloseIndex = -1;

			if (!string.IsNullOrEmpty(_textField.text) && _textField.cursorIndex > 0)
			{
				var openIndex = _textField.text.LastIndexOf(Message.LookupOpen, _textField.cursorIndex - 1);

				if (openIndex >= 0)
				{
					var closeIndex = _textField.text.LastIndexOf(Message.LookupClose, _textField.cursorIndex - 1);

					if (closeIndex < 0)
					{
						_currentOpenIndex = openIndex + 1;
						_currentCloseIndex = _textField.text.IndexOf(Message.LookupClose, _textField.cursorIndex - 1);
					}
				}
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

		#region IAutocompleteProxy Implementation

		public IAutocompleteItem Autocomplete => _currentOpenIndex < 0 ? null : _autocomplete;

		public Vector2 ControlPosition
		{
			get
			{
				var text = _currentOpenIndex < 0 ? string.Empty : _textField.text.Substring(0, _currentOpenIndex);
				var size = _measure.MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

				return _textField.worldBound.position + size;
			}
		}

		public string Variable
		{
			get
			{
				if (_currentOpenIndex < 0)
					return string.Empty;

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
