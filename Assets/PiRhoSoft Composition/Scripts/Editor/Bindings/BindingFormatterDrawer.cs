using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class BindingFormatterControl : ObjectControl<BindingFormatter>
	{
		private BindingFormatter _formatter;

		private float _preview = Mathf.PI;

		private static readonly GUIContent _previewContent = new GUIContent("Preview", "A preview of what will be displayed");
		private static readonly Label _stringFormatLabel = new Label(typeof(BindingFormatter), nameof(BindingFormatter.Format));
		private static readonly Label _formattingLabel = new Label(typeof(BindingFormatter), nameof(BindingFormatter.Formatting));
		private static readonly Label _timeFormattingLabel = new Label(typeof(BindingFormatter), nameof(BindingFormatter.TimeFormatting));
		private static readonly Label _numberFormattingLabel = new Label(typeof(BindingFormatter), nameof(BindingFormatter.NumberFormatting));
		private static readonly Label _valueFormatLabel = new Label(typeof(BindingFormatter), nameof(BindingFormatter.ValueFormat));

		public override void Setup(BindingFormatter target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_formatter = target;
		}

		public override float GetHeight(GUIContent label)
		{
			return RectHelper.LineHeight * (_formatter.Formatting == BindingFormatter.FormatType.None ? 2 : 6);
		}

		public override void Draw(Rect position, GUIContent label)
		{
			var stringFormatRect = RectHelper.TakeLine(ref position);
			var formattingRect = RectHelper.TakeLine(ref position);
			var formatRect = RectHelper.TakeLine(ref position);
			var valueFormatRect = RectHelper.TakeLine(ref position);
			var previewValueRect = RectHelper.TakeLine(ref position);
			var previewRect = RectHelper.TakeLine(ref position);

			_formatter.Format = EditorGUI.TextField(stringFormatRect, _stringFormatLabel.Content, _formatter.Format);
			_formatter.Formatting = (BindingFormatter.FormatType)EditorGUI.EnumPopup(formattingRect, _formattingLabel.Content, _formatter.Formatting);

			if (_formatter.Formatting != BindingFormatter.FormatType.None)
			{
				if (_formatter.Formatting == BindingFormatter.FormatType.Time)
				{
					_formatter.TimeFormatting = (BindingFormatter.TimeFormatType)EditorGUI.EnumPopup(formatRect, _timeFormattingLabel.Content, _formatter.TimeFormatting);
					_formatter.ValueFormat = _formatter.TimeFormatting == BindingFormatter.TimeFormatType.Custom ? EditorGUI.TextField(valueFormatRect, _valueFormatLabel.Content, _formatter.ValueFormat) : BindingFormatter.TimeFormats[(int)_formatter.TimeFormatting];
				}
				else if (_formatter.Formatting == BindingFormatter.FormatType.Number)
				{
					_formatter.NumberFormatting = (BindingFormatter.NumberFormatType)EditorGUI.EnumPopup(formatRect, _numberFormattingLabel.Content, _formatter.NumberFormatting);
					_formatter.ValueFormat = _formatter.NumberFormatting == BindingFormatter.NumberFormatType.Custom ? EditorGUI.TextField(valueFormatRect, _valueFormatLabel.Content, _formatter.ValueFormat) : BindingFormatter.NumberFormats[(int)_formatter.NumberFormatting];
				}

				if ((_formatter.Formatting == BindingFormatter.FormatType.Time && _formatter.TimeFormatting != BindingFormatter.TimeFormatType.Custom) || (_formatter.Formatting == BindingFormatter.FormatType.Number && _formatter.NumberFormatting != BindingFormatter.NumberFormatType.Custom))
					EditorGUI.LabelField(valueFormatRect, _valueFormatLabel.Content, new GUIContent(_formatter.ValueFormat));

				_preview = EditorGUI.FloatField(previewValueRect, _previewContent, _preview);

				try
				{
					var preview = _formatter.GetFormattedString(_preview);
					EditorGUI.LabelField(previewRect, " ", preview);
				}
				catch
				{
					EditorGUI.LabelField(previewRect, " ", "Invalid Format");
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(BindingFormatter))]
	public class BindingFormatterDrawer : ControlDrawer<BindingFormatterControl>
	{
	}
}
