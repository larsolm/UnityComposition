namespace PiRhoSoft.Utilities
{
	public class StretchAttribute : PropertyTraitAttribute
	{
		public const int Order = ListAttribute.Order + 10;

		public StretchAttribute() : base(Order)
		{
		}
	}
}