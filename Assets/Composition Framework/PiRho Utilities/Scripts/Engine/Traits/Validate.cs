namespace PiRhoSoft.Utilities
{
	public class ValidateAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }
		public MessageBoxType Type { get; private set; }
		public string Message { get; private set; }

		public ValidateAttribute(string method, MessageBoxType type, string message) : base(int.MaxValue - 40)
		{
			Method = method;
			Type = type;
			Message = message;
		}
	}
}
