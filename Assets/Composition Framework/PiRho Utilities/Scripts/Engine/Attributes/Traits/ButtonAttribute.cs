namespace PiRhoSoft.Utilities
{
	public class ButtonAttribute : PropertyTraitAttribute
	{
		public const int Order = 1;

		public string Method { get; private set; }

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public TraitLocation Location { get; set; }

		public ButtonAttribute(string method) : base(Order)
		{
			Method = method;
		}
	}
}