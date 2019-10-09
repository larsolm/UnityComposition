using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceAutocompleteControl : VisualElement, IAutocompleteProxy
	{
		public const string Stylesheet = "Variables/VariableReference/VariableReferenceAutocompleteStyle.uss";
		public const string UssClassName = "pirho-variable-reference-autocomplete";
		public const string TextUssClassName = UssClassName + "__text";

		private VariableReferenceControl _control;
		private AutocompleteControl _autocompleteControl;
		private TextField _textField;

		public VariableReferenceAutocompleteControl(VariableReferenceControl control)
		{
			_control = control;
			_textField = new TextField();
			_textField.AddToClassList(TextUssClassName);
			_textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.Q(className: TextField.inputUssClassName).RegisterCallback<MouseUpEvent>(evt => RefreshAutocomplete());
			_autocompleteControl = new AutocompleteControl(this, _textField);

			Add(_textField);
			Add(_autocompleteControl);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);

			Refresh();
		}

		public void Refresh()
		{
			_textField.SetValueWithoutNotify(_control.Value.Variable);
			RefreshAutocomplete();
		}

		private void RefreshAutocomplete()
		{
			_autocompleteControl.Refresh();
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			// Schedule these so cursorIndex is correct
			if (evt.keyCode == KeyCode.LeftArrow)
				schedule.Execute(RefreshAutocomplete).StartingIn(0);
			else if (evt.keyCode == KeyCode.RightArrow)
				schedule.Execute(RefreshAutocomplete).StartingIn(0);
		}

		#region IAutocompleteProxy Implementation

		public IAutocompleteItem Autocomplete => _control.Autocomplete;
		public Vector2 ControlPosition => new Vector2(_textField.worldBound.position.x, _textField.worldBound.yMax);

		public string Variable
		{
			get => _textField.text;
			set => _textField.value = value;
		}
		public int Cursor
		{
			get => _textField.cursorIndex;
			set => _textField.SelectRange(value, value);
		}

		#endregion
	}
}
