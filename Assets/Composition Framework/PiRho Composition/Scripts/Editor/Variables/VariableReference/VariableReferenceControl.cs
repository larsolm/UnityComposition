using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
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
		public IAutocompleteItem Autocomplete { get; private set; }

		private IconButton _modeToggle;
		private VariableReferencePopupControl _simpleControl;
		private VariableReferenceTextControl _advancedControl;

		private bool _advancedMode = false;

		public VariableReferenceControl(VariableReference value, IAutocompleteItem autocomplete)
		{
			Value = value;
			Autocomplete = new TreeAutocomplete1(string.Empty, false, false);

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

	public class TreeAutocomplete1 : AutocompleteItem
	{
		public TreeAutocomplete1(string name, bool allowsCustomFields, bool isIndexable)
		{
			Name = name;
			AllowsCustomFields = allowsCustomFields;
			IsIndexable = isIndexable;

			Fields = new List<IAutocompleteItem>
			{
				new TreeAutocomplete1("One", true, true),
				new TreeAutocomplete1("Two", false, true),
				new TreeAutocomplete1("Three", true, false),
				new TreeAutocomplete1("Four", false, false),
				new TreeAutocomplete2("Five", true, true),
				new TreeAutocomplete2("Six", false, true),
				new TreeAutocomplete2("Seven", true, false),
				new TreeAutocomplete2("Eight", false, false)
			};
		}

		public override void Setup(object obj)
		{
		}
	}

	public class TreeAutocomplete2 : AutocompleteItem
	{
		public TreeAutocomplete2(string name, bool allowsCustomFields, bool isIndexable)
		{
			Name = name;
			AllowsCustomFields = allowsCustomFields;
			IsIndexable = isIndexable;
		}

		public override void Setup(object obj)
		{
		}
	}
}
