namespace PiRhoSoft.Composition
{
	public class IsolatedStore : VariableStore
	{
		private IVariableStore _parent;

		public IsolatedStore(IVariableStore parent)
		{
			_parent = parent;
		}

		public override SetVariableResult SetVariable(string name, Variable value)
		{
			// the _parent store might allow arbitrary adds so only set it on the parent if it already exists (as
			// opposed to checking for a set failure)
			var exists = !_parent.GetVariable(name).IsEmpty;

			if (exists)
				return _parent.SetVariable(name, value);
			else
				return SetVariable(name, value, true);
		}
	}
}
