namespace PiRhoSoft.Utilities
{
	public class DictionaryAttribute : PropertyTraitAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public string EmptyLabel = null;

		public DictionaryAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}