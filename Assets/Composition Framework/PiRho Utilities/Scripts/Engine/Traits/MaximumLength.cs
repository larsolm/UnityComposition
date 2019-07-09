namespace PiRhoSoft.Utilities.Engine
{
	public class MaximumLengthAttribute : PropertyTraitAttribute
	{
		public int Length { get; private set; }

		public MaximumLengthAttribute(int length) : base(int.MaxValue - 120)
		{
			Length = length;
		}
	}
}
