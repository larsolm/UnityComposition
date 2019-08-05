using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ComboBoxControl : VisualElement
	{
		private const string _invalidTypeWarning = "(PUCBCIT) failed to set value on EnumButtonsControl: attempted to set value '{0}' of enum '{1}' but the control is using enum type '{2}'";

		public const string Stylesheet = "ComboBox/ComboBoxStyle.uss";
		public static readonly string UssClassName = "pirho-combo-box";
		public static readonly string TextUssClassName = UssClassName + "__text";
		public static readonly string ButtonUssClassName = UssClassName + "__button";

		private static readonly Icon _dropdownIcon = Icon.BuiltIn("icon dropdown");

		public string Value { get; private set; }
		public List<string> Options { get; private set; }
		public TextField TextField { get; private set; } // Public so delayed can be set

		private IconButton _dropdownButton;
		private GenericMenu _menu;

		public ComboBoxControl(string value, List<string> options)
		{
			Value = value;
			Options = options;

			Setup();
			Refresh();

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void SetValueWithoutNotify(string value)
		{
			if (value != Value)
			{
				Value = value;
				Refresh();
			}
		}

		private void Setup()
		{
			TextField = new TextField();
			TextField.AddToClassList(TextUssClassName);
			TextField.RegisterValueChangedCallback(evt => Value = evt.newValue);

			_dropdownButton = new IconButton(_dropdownIcon.Texture, "Show the combo box options", OpenDropdown);
			_dropdownButton.AddToClassList(ButtonUssClassName);

			_menu = new GenericMenu();

			foreach (var option in Options)
				_menu.AddItem(new GUIContent(option), false, () => SelectItem(option));

			Add(TextField);
			Add(_dropdownButton);
		}

		private void Refresh()
		{
			TextField.SetValueWithoutNotify(Value);
		}

		private void OpenDropdown()
		{
			_menu.DropDown(_dropdownButton.worldBound);
		}

		private void SelectItem(string option)
		{
			var previous = Value;
			SetValueWithoutNotify(option);
			this.SendChangeEvent(previous, option);
		}
	}
}