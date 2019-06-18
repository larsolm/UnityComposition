namespace PiRhoSoft.PargonUtilities.Engine
{
	public class MaximumAttribute : PropertyTraitAttribute
	{
		private const int _order = int.MaxValue - 120;

		public float Maximum { get; private set; }

		public MaximumAttribute(float maximum) : base(_order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(int maximum) : base(_order)
		{
			Maximum = maximum;
		}
	}
}
