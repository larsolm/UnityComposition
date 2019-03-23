namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableResolver : ListVariableResolver
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (owner.HasList)
				return base.Lookup(owner, lookup);
			else
				return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (owner.HasList)
				return base.Apply(ref owner, lookup, value);
			else
				return SetVariableResult.NotFound;
		}
	}
}
