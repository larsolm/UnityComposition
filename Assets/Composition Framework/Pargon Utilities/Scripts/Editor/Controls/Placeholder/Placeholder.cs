using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class Placeholder<T> : VisualElement
	{
		private const string _invalidChildrenWarning = "(PICPIC) Unable to find child text input field Placeholder";

		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Placeholder/Placeholder.uss";

		public string Text { get; private set; }

		private TextInputBaseField<T> _input;
		private Label _placeholder;

		public void Setup(string text, SerializedProperty property)
		{
			Add(new BindablePropertyElement(property, null));
			Setup(text);
		}

		public void Setup(string text)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			Text = text;

			AddToClassList("placeholder");

			_input = this.Query<TextInputBaseField<T>>();
			_input.RegisterValueChangedCallback(e => TextChanged());
			_input.AddToClassList("input");
			_placeholder = new Label(text);
			_placeholder.AddToClassList("text");

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
