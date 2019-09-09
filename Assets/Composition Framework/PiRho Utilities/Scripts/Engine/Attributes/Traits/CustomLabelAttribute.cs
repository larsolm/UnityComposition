namespace PiRhoSoft.Utilities
{
	public class CustomLabelAttribute : PropertyTraitAttribute
	{
		public string Label { get; set; }
		public string Method { get; set; }

		public CustomLabelAttribute(string label = null, string method = null) : base(PerContainerPhase, 0)
		{
			Label = label;
			Method = method;
		}
	}
}