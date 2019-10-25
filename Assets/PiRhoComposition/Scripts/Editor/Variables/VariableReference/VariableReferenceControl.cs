using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceControl : VisualElement
	{
		public const string Stylesheet = "Variables/VariableReference/VariableReferenceStyle.uss";
		public const string UssClassName = "pirho-variable-reference";
		public const string InvalidModeUssClassName = UssClassName + "--invalid";
		public const string AdvancedModeUssClassName = UssClassName + "--advanced";
		public const string ModeUssClassName = UssClassName + "__mode";
		public const string SimpleUssClassName = UssClassName + "__simple";
		public const string AdvancedUssClassName = UssClassName + "__advanced";

		private const string _validTooltip = "Toggle editing as dropdown (simple) vs text (advanced)";
		private const string _invalidTooltip = "Must fix syntax errors before toggling to dropdown mode";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableReference Value { get; private set; }
		public IAutocompleteItem Autocomplete { get; private set; }

		private readonly IconButton _modeToggle;
		private readonly VariableReferencePopupControl _simpleControl;
		private readonly VariableReferenceAutocompleteControl _advancedControl;

		private bool _advancedMode = false;

		public VariableReferenceControl(VariableReference value, IAutocompleteItem autocomplete)
		{
			Value = value;
			Autocomplete = autocomplete;

			_modeToggle = new IconButton(_modeIcon.Texture, _validTooltip, () =>
			{
				_advancedMode = !_advancedMode;
				Refresh();
			});

			_modeToggle.AddToClassList(ModeUssClassName);

			_simpleControl = new VariableReferencePopupControl(this);
			_simpleControl.AddToClassList(SimpleUssClassName);

			_advancedControl = new VariableReferenceAutocompleteControl(this);
			_advancedControl.AddToClassList(AdvancedUssClassName);

			Add(_modeToggle);
			Add(_simpleControl);
			Add(_advancedControl);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
			Refresh();
		}

		public void SetValueWithoutNotify(string value)
		{
			Value.Variable = value;
			Refresh();
		}

		private void Refresh()
		{
			if (!Value.IsValid)
				_advancedMode = true;

			if (_advancedMode)
				_advancedControl.Refresh();
			else
				_simpleControl.Refresh();

			_modeToggle.tooltip = Value.IsValid ? _validTooltip : _invalidTooltip;

			_modeToggle.SetEnabled(Value.IsValid);
			_simpleControl.SetEnabled(!_advancedMode);
			_advancedControl.SetEnabled(_advancedMode);

			EnableInClassList(InvalidModeUssClassName, !Value.IsValid);
			EnableInClassList(AdvancedModeUssClassName, _advancedMode);
		}
	}
}
