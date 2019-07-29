using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class IsolatedVariableCollection : IVariableCollection
	{
		private IVariableCollection _parent;
		private VariableStore _self = new VariableStore();

		public IsolatedVariableCollection(IVariableCollection parent)
		{
			_parent = parent;
		}

		public IReadOnlyList<string> VariableNames { get; }

		public Variable GetVariable(string name)
		{
			var self = _self.GetVariable(name);

			return self.IsEmpty
				? _parent.GetVariable(name)
				: self;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			// the _parent store might allow arbitrary adds so only set it on the parent if it already exists (as
			// opposed to checking for a set failure)
			var exists = !_parent.GetVariable(name).IsEmpty;

			if (exists)
				return _parent.SetVariable(name, value);
			else
				return _self.SetVariable(name, value);
		}
	}
}
