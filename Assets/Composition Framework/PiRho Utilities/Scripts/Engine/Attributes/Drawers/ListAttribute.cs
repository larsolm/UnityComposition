namespace PiRhoSoft.Utilities
{
	public class ListAttribute : PropertyTraitAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = true;
		public string EmptyLabel = null;

		public string AddCallback = null;
		public string RemoveCallback = null;
		public string ReorderCallback = null;

		public ListAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}