namespace PiRhoSoft.CompositionEngine
{
	public abstract class VariableResolver
	{
		public abstract VariableValue Lookup(VariableValue owner, string lookup);
		public abstract SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value);
	}
}
