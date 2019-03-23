namespace PiRhoSoft.CompositionEngine
{
	public class BoolVariableResolver : VariableHandler
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return SetVariableResult.NotFound;
		}
	}
}
