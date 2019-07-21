namespace PiRhoSoft.Utilities
{
	public class ValidateAttribute : PropertyTraitAttribute
	{
		public const int Order = 40;

		public string Method { get; private set; }
		public string Message { get; private set; }
		public TraitMessageType Type { get; private set; }

		public ValidateAttribute(string method, string message, TraitMessageType type = TraitMessageType.Warning) : base(Order)
		{
			Method = method;
			Type = type;
			Message = message;
		}
	}
}