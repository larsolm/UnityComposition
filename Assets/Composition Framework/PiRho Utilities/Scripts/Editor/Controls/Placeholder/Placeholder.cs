using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Placeholder<T> : VisualElement
	{
		public const string StyleSheetPath = Utilities.AssetPath + "Controls/Placeholder/Placeholder.uss";
		public const string UssClassName = "pirho-placeholder";
		public const string UssInputClassName = UssClassName + "__input";
		public const string UssTextClassName = UssClassName + "__text";

		private TextInputBaseField<T> _input;
		private Label _placeholder;

		public Placeholder(string text, SerializedProperty property)
		{
			Add(new ElementHelper.BindablePropertyElement(property, null, null));
			Setup(text);
		}

		public Placeholder(string text, TextInputBaseField<T> input)
		{
			Add(input);
			Setup(text);
		}

		private void Setup(string text)
		{
			ElementHelper.AddStyleSheet(this, StyleSheetPath);

			AddToClassList(UssClassName);

			_input = this.Q<TextInputBaseField<T>>();
			_input.RegisterValueChangedCallback(e => TextChanged());
			_input.AddToClassList(UssInputClassName);

			_placeholder = new Label(text);
			_placeholder.AddToClassList(UssTextClassName);

			_input.Add(_placeholder);

			TextChanged();
		}

		private void TextChanged()
		{
			var empty = string.IsNullOrEmpty(_input.text);

			ElementHelper.SetVisible(_placeholder, empty);
		}
	}
}
