using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class AggregateVariableCollection : IVariableMap
	{
		private IVariableMap[] _stores;

		public AggregateVariableCollection(params IVariableMap[] variables)
		{
			_stores = variables;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => VariableDictionary.EmptyNames;
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
