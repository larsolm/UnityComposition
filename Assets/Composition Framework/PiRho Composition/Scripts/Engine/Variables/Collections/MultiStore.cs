using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class MultiStore : IVariableCollection
	{
		private IVariableCollection[] _stores;

		public MultiStore(params IVariableCollection[] stores)
		{
			_stores = stores;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => VariableStore.EmptyNames;
		}

		public Variable GetVariable(string name)
		{
			return Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			return SetVariableResult.NotFound;
		}
	}
}