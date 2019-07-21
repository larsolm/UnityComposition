namespace PiRhoSoft.Utilities
{
	public class ReadOnlyAttribute : PropertyTraitAttribute
	{
		public const int Order = 10;

		public ReadOnlyAttribute() : base(Order)
		{
		}
	}
}