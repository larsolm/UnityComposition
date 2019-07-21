namespace PiRhoSoft.Utilities
{
	public class MinimumAttribute : PropertyTraitAttribute
	{
		public const int Order = 130;

		public float Minimum { get; private set; }

		public MinimumAttribute(float minimum) : base(Order)
		{
			Minimum = minimum;
		}

		public MinimumAttribute(int minimum) : base(Order)
		{
			Minimum = minimum;
		}
	}
}