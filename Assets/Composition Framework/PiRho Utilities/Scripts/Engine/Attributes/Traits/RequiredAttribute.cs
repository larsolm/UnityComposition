namespace PiRhoSoft.Utilities
{
	public class RequiredAttribute : PropertyTraitAttribute
	{
		public const int Order = 50;

		public string Message { get; private set; }
		public TraitMessageType Type { get; private set; }

		public RequiredAttribute(string message, TraitMessageType type = TraitMessageType.Warning) : base(Order)
		{
			Message = message;
			Type = type;
		}
	}
}