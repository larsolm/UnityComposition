using PiRhoSoft.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class BindingFormatterControl : VisualElement
	{
		public BindingFormatter Value;

		private readonly VisualElement _none;
		private readonly VisualElement _time;
		private readonly VisualElement _number;
		private readonly VisualElement _value;
		private readonly VisualElement _preview;
		private readonly TextField _valueText;
		private readonly FloatField _previewNumber;
		private readonly Label _previewLabel;

		public BindingFormatterControl(BindingFormatter value)
		{
			Value = value;

			var formatField = new TextField("Label Format") { tooltip = "The format of the displayed string" };
			var formatContainer = new FieldContainer("Format Type", "The method to format the number by");

			_none = new VisualElement();
			_time = new FieldContainer("Time Format", "The way to format the time");
			_number = new FieldContainer("Numer Format", "The way to format the number");
			_value = new FieldContainer("Value Format", "The format string to be used to format the number");
			_preview = new FieldContainer("Preview", "A preview of what the the format will display");

			//formatContainer.Add(new EnumButtons(owner, () => _format.Formatting, value =>
			//{
			//	_format.Formatting = (BindingFormatter.FormatType)value;
			//
			//	if (_format.Formatting == BindingFormatter.FormatType.Time && _format.TimeFormatting != BindingFormatter.TimeFormatType.Custom)
			//		_format.ValueFormat = BindingFormatter.TimeFormats[(int)_format.TimeFormatting];
			//
			//	if (_format.Formatting == BindingFormatter.FormatType.Number && _format.NumberFormatting != BindingFormatter.NumberFormatType.Custom)
			//		_format.ValueFormat = BindingFormatter.NumberFormats[(int)_format.NumberFormatting];
			//
			//	Refresh();
			//}));

			//_time.Add(new EnumDropdown<BindingFormatter.TimeFormatType>(_format.TimeFormatting, owner, () => (int)_format.TimeFormatting, value =>
			//{
			//	_format.TimeFormatting = (BindingFormatter.TimeFormatType)value;
			//
			//	if (_format.TimeFormatting != BindingFormatter.TimeFormatType.Custom)
			//		_format.ValueFormat = BindingFormatter.TimeFormats[(int)_format.TimeFormatting];
			//
			//	Refresh();
			//}));

			//_number.Add(new EnumDropdown<BindingFormatter.NumberFormatType>(_format.NumberFormatting, owner, () => (int)_format.NumberFormatting, value =>
			//{
			//	_format.NumberFormatting = (BindingFormatter.NumberFormatType)value;
			//
			//	if (_format.NumberFormatting != BindingFormatter.NumberFormatType.Custom)
			//		_format.ValueFormat = BindingFormatter.NumberFormats[(int)_format.NumberFormatting];
			//
			//	Refresh();
			//}));

			_valueText = new TextField() { value = Value.ValueFormat };
			_valueText.RegisterValueChangedCallback(evt =>
			{
				Value.ValueFormat = evt.newValue;
				Refresh();
			});

			_previewLabel = new Label();
			_previewNumber = new FloatField() { value = Mathf.PI };
			_previewNumber.RegisterValueChangedCallback(evt =>
			{
				try
				{
					var preview = Value.GetFormattedString(evt.newValue);
					_previewLabel.text = preview;
				}
				catch
				{
					_previewLabel.text = "Invalid Format";
				}
			});

			Add(formatContainer);
			Add(_none);

			_none.Add(_time);
			_none.Add(_number);
			_none.Add(_value);
			_none.Add(_preview);

			_value.Add(_previewLabel);
			_preview.Add(_previewNumber);
			_preview.Add(_previewLabel);

			Refresh();
		}

		public void SetValueWithoutNotify(BindingFormatter value)
		{
			Value = value;
			Refresh();
		}

		private void Refresh()
		{
			_none.SetDisplayed(Value.Formatting != BindingFormatter.FormatType.None);
			_time.SetDisplayed(Value.Formatting == BindingFormatter.FormatType.Time);
			_number.SetDisplayed(Value.Formatting == BindingFormatter.FormatType.Number);

			_valueText.SetValueWithoutNotify(Value.ValueFormat);
			_value.SetEnabled((Value.Formatting == BindingFormatter.FormatType.Time && Value.TimeFormatting == BindingFormatter.TimeFormatType.Custom)
				|| (Value.Formatting == BindingFormatter.FormatType.Number && Value.NumberFormatting == BindingFormatter.NumberFormatType.Custom));
		}
	}
}
