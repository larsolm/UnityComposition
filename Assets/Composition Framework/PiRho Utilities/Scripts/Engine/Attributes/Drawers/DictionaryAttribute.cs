namespace PiRhoSoft.Utilities
{
	public class DictionaryAttribute : PropertyTraitAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = false;
		public string EmptyLabel = null;

		public string AddCallback = null;
		public string RemoveCallback = null;
		public string ReorderCallback = null;

		public DictionaryAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}