namespace PiRhoSoft.Utilities.Engine
{
	public class ReadOnlyAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }

		public ReadOnlyAttribute() : base(int.MaxValue - 10)
		{
		}
	}
}
