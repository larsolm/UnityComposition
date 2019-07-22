using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class BindingFormatterElement : VisualElement
	{
		private static readonly string _formatTooltip = ElementHelper.GetTooltip(typeof(BindingFormatter), nameof(BindingFormatter.Format));
		private static readonly string _formattingTooltip = ElementHelper.GetTooltip(typeof(BindingFormatter), nameof(BindingFormatter.Formatting));
		private static readonly string _timeFormattingTooltip = ElementHelper.GetTooltip(typeof(BindingFormatter), nameof(BindingFormatter.TimeFormatting));
		private static readonly string _numberFormattingTooltip = ElementHelper.GetTooltip(typeof(BindingFormatter), nameof(BindingFormatter.NumberFormatting));
		private static readonly string _valueFormatTooltip = ElementHelper.GetTooltip(typeof(BindingFormatter), nameof(BindingFormatter.ValueFormat));

		private readonly BindingFormatter _format;

		private readonly VisualElement _none;
		private readonly VisualElement _time;
		private readonly VisualElement _number;
		private readonly VisualElement _value;
		private readonly VisualElement _preview;
		private readonly TextField _valueText;
		private readonly FloatField _previewNumber;
		private readonly Label _previewLabel;

		public BindingFormatterElement(SerializedProperty property) : this(property.serializedObject.targetObject, PropertyHelper.GetObject<BindingFormatter>(property)) { }

		public BindingFormatterElement(Object owner, BindingFormatter formatter)
		{
			_format = formatter;

			var formatField = new TextField("Label Format") { tooltip = _formatTooltip };
			var formatContainer = ElementHelper.CreatePropertyContainer("Format Type", _formattingTooltip);

			_none = new VisualElement();
			_time = ElementHelper.CreatePropertyContainer("Time Format", _timeFormattingTooltip);
			_number = ElementHelper.CreatePropertyContainer("Numer Format", _numberFormattingTooltip);
			_value = ElementHelper.CreatePropertyContainer("Value Format", _valueFormatTooltip);
			_preview = ElementHelper.CreatePropertyContainer("Preview", "A preview of what the the format will display");

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

			_valueText = new TextField() { value = _format.ValueFormat };

			ElementHelper.Bind(this, _valueText, owner, () => _format.ValueFormat, value =>
			{
				_format.ValueFormat = value;
				Refresh();
			});

			_previewLabel = new Label();
			_previewNumber = new FloatField() { value = Mathf.PI };
			_previewNumber.RegisterValueChangedCallback(evt =>
			{
				try
				{
					var preview = _format.GetFormattedString(evt.newValue);
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

		private void Refresh()
		{
			ElementHelper.SetVisible(_none, _format.Formatting != BindingFormatter.FormatType.None);
			ElementHelper.SetVisible(_time, _format.Formatting == BindingFormatter.FormatType.Time);
			ElementHelper.SetVisible(_number, _format.Formatting == BindingFormatter.FormatType.Number);

			_valueText.SetValueWithoutNotify(_format.ValueFormat);
			_value.SetEnabled((_format.Formatting == BindingFormatter.FormatType.Time && _format.TimeFormatting == BindingFormatter.TimeFormatType.Custom)
				|| (_format.Formatting == BindingFormatter.FormatType.Number && _format.NumberFormatting == BindingFormatter.NumberFormatType.Custom));
		}
	}
}
