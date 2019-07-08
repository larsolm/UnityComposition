namespace PiRhoSoft.Composition.Engine
{
	public class WritableStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariable(name, value, false);
		}
	}
}
