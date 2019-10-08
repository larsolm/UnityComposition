using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class MessageControl : VisualElement, IAutocompleteProxy
	{
		public const string Stylesheet = "Variables/Message/MessageStyle.uss";
		public const string UssClassName = "pirho-message";
		public const string TextUssClassName = "pirho-message__text";

		public Message Value { get; private set; }

		private AutocompleteControl _autocompleteControl;
		private readonly IAutocompleteItem _autocomplete;
		private int _currentOpenIndex = -1;
		private int _currentCloseIndex = -1;

		private TextElement _measure;

		public MessageControl(Message value, IAutocompleteItem autocomplete)
		{
			Value = value;

			TextField = new TextField { multiline = true };
			TextField.AddToClassList(TextUssClassName);
			TextField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			TextField.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
			TextField.Q(className: TextField.inputUssClassName).RegisterCallback<MouseUpEvent>(OnMouseUp);

			_autocomplete = autocomplete;
			_autocompleteControl = new AutocompleteControl(this);
			_measure = new TextElement();

			Add(TextField);
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
			TextField.SetValueWithoutNotify(Value.Text);
			RefreshAutocomplete();
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

		private void OnMouseUp(MouseUpEvent evt)
		{
			RefreshAutocomplete();
		}

		private void RefreshAutocomplete()
		{
			_currentOpenIndex = -1;
			_currentCloseIndex = -1;

			if (!string.IsNullOrEmpty(TextField.text) && TextField.cursorIndex > 0)
			{
				var openIndex = TextField.text.LastIndexOf(Message.LookupOpen, TextField.cursorIndex - 1);

				if (openIndex >= 0)
				{
					var closeIndex = TextField.text.LastIndexOf(Message.LookupClose, TextField.cursorIndex - 1);

					if (closeIndex < 0)
					{
						_currentOpenIndex = openIndex + 1;
						_currentCloseIndex = TextField.text.IndexOf(Message.LookupClose, TextField.cursorIndex - 1);
					}
				}
			}

			_autocompleteControl.Refresh();
		}

		#region IAutocompleteProxy Implementation

		public TextField TextField { get; private set; }
		public IAutocompleteItem Autocomplete => _currentOpenIndex < 0 ? null : _autocomplete;

		public string Variable => _currentOpenIndex < 0 ? string.Empty : _currentCloseIndex < 0 ? TextField.text.Substring(_currentOpenIndex) : TextField.text.Substring(_currentOpenIndex, _currentCloseIndex - _currentOpenIndex);
		public int Cursor => TextField.cursorIndex - _currentOpenIndex;

		public void SetVariable(string value)
		{
			var previous = TextField.text;
			var prefix = _currentCloseIndex < 0 ? Value.Text.Remove(_currentOpenIndex) : Value.Text.Remove(_currentOpenIndex, _currentCloseIndex - _currentOpenIndex);
			var next = prefix.Insert(_currentOpenIndex, value);

			TextField.value = next;
		}

		public void SetCursor(int cursor)
		{
			var index = cursor + _currentOpenIndex + 1;
			TextField.SelectRange(index, index);
		}

		public Vector2 GetPosition()
		{
			var text = _currentOpenIndex < 0 ? string.Empty : TextField.text.Substring(0, _currentOpenIndex);
			var size = _measure.MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

			return TextField.worldBound.position + size;
		}

		#endregion
	}
}
