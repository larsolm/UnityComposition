namespace PiRhoSoft.Utilities
{
	public class ListAttribute : PropertyTraitAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = true;
		public string EmptyLabel = null;

		public ListAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}