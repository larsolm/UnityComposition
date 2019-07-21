namespace PiRhoSoft.Utilities
{
	public class MaximumLengthAttribute : PropertyTraitAttribute
	{
		public const int Order = 120;

		public int Length { get; private set; }

		public MaximumLengthAttribute(int length) : base(Order)
		{
			Length = length;
		}
	}
}