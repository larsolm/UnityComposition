namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableResolver : VariableResolver
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (owner.TryGetReference(out IIndexedVariableStore store))
			{
				if (int.TryParse(lookup, out var index))
					return VariableValue.CreateReference(store.GetItem(index));
			}

			return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (owner.TryGetReference(out IIndexedVariableStore store))
			{
				if (int.TryParse(lookup, out var index))
				{
					if (value.HasReference)
						return store.SetItem(index, value.Reference) ? SetVariableResult.Success : SetVariableResult.ReadOnly;
					else
						return SetVariableResult.TypeMismatch;
				}
			}

			return SetVariableResult.NotFound;
		}
	}
}
