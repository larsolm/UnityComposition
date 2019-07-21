using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PlaceholderControl : Label
	{
		public const string Stylesheet = "Placeholder/PlaceholderStyle.uss";
		public static readonly string UssClassName = "pirho-placeholder";

		public PlaceholderControl(string text, TextField field)
		{
			void UpdateDisplayed() => this.SetDisplayed(string.IsNullOrEmpty(field.text));

			this.text = text;
			AddToClassList(UssClassName);
			this.AddStyleSheet(Stylesheet);
			UpdateDisplayed();

			field.RegisterCallback<ChangeEvent<string>>(evt => UpdateDisplayed());
		}
	}
}