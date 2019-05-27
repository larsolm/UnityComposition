namespace PiRhoSoft.UtilityEngine
{
	public class DisableInInspectorAttribute : PropertyScopeAttribute
	{
		public const int DefaultOrder = int.MaxValue - 10;

		public DisableInInspectorAttribute() : base(DefaultOrder) { }
	}
}
