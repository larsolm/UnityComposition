namespace PiRhoSoft.Utilities
{
	public enum MessageBoxType
	{
		Info,
		Warning,
		Error
	}

	public class RequiredAttribute : PropertyTraitAttribute
	{
		public MessageBoxType Type { get; private set; }
		public string Message { get; private set; }

		public RequiredAttribute(MessageBoxType type, string message) : base(int.MaxValue - 50)
		{
			Type = type;
			Message = message;
		}
	}
}
