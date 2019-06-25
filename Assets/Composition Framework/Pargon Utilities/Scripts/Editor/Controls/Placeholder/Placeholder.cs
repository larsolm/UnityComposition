using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class Placeholder<T> : VisualElement
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Placeholder/Placeholder.uss";
		private const string _ussPlaceholder = "pargon-placeholder";
		private const string _ussInput = "input";
		private const string _ussText = "text";

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
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			AddToClassList(_ussPlaceholder);

			_input = this.Q<TextInputBaseField<T>>();
			_input.RegisterValueChangedCallback(e => TextChanged());
			_input.AddToClassList(_ussInput);

			_placeholder = new Label(text);
			_placeholder.AddToClassList(_ussText);

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
