namespace PiRhoSoft.Utilities
{
	public class MaximumAttribute : PropertyTraitAttribute
	{
		public const int Order = 120;

		public float Maximum { get; private set; }

		public MaximumAttribute(float maximum) : base(Order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(int maximum) : base(Order)
		{
			Maximum = maximum;
		}
	}
}