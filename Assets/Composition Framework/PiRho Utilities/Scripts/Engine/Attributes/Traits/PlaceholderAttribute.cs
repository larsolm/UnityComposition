namespace PiRhoSoft.Utilities
{
	public class PlaceholderAttribute : PropertyTraitAttribute
	{
		public const int Order = 1000;

		public string Text { get; private set; }

		public PlaceholderAttribute(string text) : base(Order)
		{
			Text = text;
		}
	}
}