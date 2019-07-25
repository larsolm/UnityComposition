namespace PiRhoSoft.Composition
{
	public class ReadOnlyStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, Variable value)
		{
			if (Map.TryGetValue(name, out int index))
				return SetVariableResult.ReadOnly;
			else
				return SetVariableResult.NotFound;
		}
	}
}
