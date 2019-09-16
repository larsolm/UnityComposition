﻿using PiRhoSoft.Utilities;
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

		private static readonly string _validTooltip = "Toggle editing as dropdown (simple) vs text (advanced)";
		private static readonly string _invalidTooltip = "Must fix syntax errors before toggling to dropdown mode";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableReference Value { get; private set; }
		public AutocompleteSource Source { get; private set; }

		private IconButton _modeToggle;
		private VariableReferencePopupControl _simpleControl;
		private VariableReferenceTextControl _advancedControl;

		private bool _advancedMode = false;

		public VariableReferenceControl(VariableReference value, AutocompleteSource source)
		{
			Value = value;
			Source = new TreeAutocomplete();

			_modeToggle = new IconButton(_modeIcon.Texture, _validTooltip, () =>
			{
				_advancedMode = !_advancedMode;
				Refresh();
			});

			_modeToggle.AddToClassList(ModeUssClassName);

			_simpleControl = new VariableReferencePopupControl(this);
			_simpleControl.AddToClassList(SimpleUssClassName);
			_simpleControl.RegisterCallback<ChangeEvent<VariableReference>>(evt => Refresh());

			_advancedControl = new VariableReferenceTextControl(this);
			_advancedControl.AddToClassList(AdvancedUssClassName);
			_advancedControl.RegisterCallback<ChangeEvent<VariableReference>>(evt => Refresh());

			Add(_modeToggle);
			Add(_simpleControl);
			Add(_advancedControl);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);

			Refresh();
		}

		public void SetValueWithoutNotify(VariableReference value)
		{
			Value = value;
			Refresh();
		}

		private void Refresh()
		{
			EnableInClassList(InvalidModeUssClassName, !Value.IsValid);
			EnableInClassList(AdvancedModeUssClassName, _advancedMode);

			if (!Value.IsValid)
				_advancedMode = true;

			_advancedControl.Refresh();
			_simpleControl.Refresh();

			_modeToggle.tooltip = Value.IsValid ? _validTooltip : _invalidTooltip;

			_modeToggle.SetEnabled(Value.IsValid);
			_simpleControl.SetEnabled(!_advancedMode);
			_advancedControl.SetEnabled(_advancedMode);
		}
	}
}