namespace PiRhoSoft.Utilities
{
	public class ChangeTriggerAttribute : PropertyTraitAttribute
	{
		public const int Order = ListAttribute.Order + 1;

		public string Method { get; private set; }

		public ChangeTriggerAttribute(string method) : base(Order)
		{
			Method = method;
		}
	}
}