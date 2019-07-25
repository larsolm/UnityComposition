namespace PiRhoSoft.Composition
{
	public class WritableStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, Variable value)
		{
			return SetVariable(name, value, false);
		}
	}
}
