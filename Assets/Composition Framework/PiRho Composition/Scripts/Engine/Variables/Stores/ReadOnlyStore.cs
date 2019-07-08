namespace PiRhoSoft.Composition.Engine
{
	public class ReadOnlyStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (Map.TryGetValue(name, out int index))
				return SetVariableResult.ReadOnly;
			else
				return SetVariableResult.NotFound;
		}
	}
}
