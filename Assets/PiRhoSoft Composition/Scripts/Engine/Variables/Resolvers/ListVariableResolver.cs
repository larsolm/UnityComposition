namespace PiRhoSoft.CompositionEngine
{
	public class ListVariableResolver : VariableResolver
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup.Length == 1 && lookup[0] == '#')
				return VariableValue.Create(owner.List.Count);
			else if (int.TryParse(lookup, out var index) && index >= 0 && index < owner.List.Count)
				return owner.List.GetVariable(index);
			else
				return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (int.TryParse(lookup, out var index) && index >= 0 && index < owner.List.Count)
				return owner.List.SetVariable(index, value);
			else if (lookup.Length == 1 && lookup[0] == '#')
				return SetVariableResult.ReadOnly;
			else
				return SetVariableResult.NotFound;
		}
	}
}
