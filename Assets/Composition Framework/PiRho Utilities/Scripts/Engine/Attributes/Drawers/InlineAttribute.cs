namespace PiRhoSoft.Utilities
{
	public class InlineAttribute : PropertyTraitAttribute
	{
		public bool ShowMemberLabels { get; private set; }

		public InlineAttribute(bool showMemberLabels = false) : base(FieldPhase, 0)
		{
			ShowMemberLabels = showMemberLabels;
		}
	}
}