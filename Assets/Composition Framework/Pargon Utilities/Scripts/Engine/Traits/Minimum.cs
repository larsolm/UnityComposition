namespace PiRhoSoft.PargonUtilities.Engine
{
	public class MinimumAttribute : PropertyTraitAttribute
	{
		private const int _order = int.MaxValue - 130;

		public float Minimum { get; private set; }

		public MinimumAttribute(float minimum) : base(_order)
		{
			Minimum = minimum;
		}

		public MinimumAttribute(int minimum) : base(_order)
		{
			Minimum = minimum;
		}
	}
}
