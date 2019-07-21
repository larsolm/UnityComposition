namespace PiRhoSoft.Utilities
{
	public class CustomLabelAttribute : PropertyTraitAttribute
	{
		public const int Order = 50;

		public string Label { get; set; }
		public string Method { get; set; }

		public CustomLabelAttribute(string label = null, string method = null) : base(Order)
		{
			Label = label;
			Method = method;
		}
	}
}