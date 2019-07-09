namespace PiRhoSoft.Utilities.Engine
{
	public class ChangeTriggerAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }

		public ChangeTriggerAttribute(string method) : base(int.MaxValue)
		{
			Method = method;
		}
	}
}
