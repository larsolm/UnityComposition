using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class MappedVariableCollection : IVariableCollection, IVariableArray
	{
		private object _owner;
		private VariableMap _map;

		public MappedVariableCollection(object owner)
		{
			_owner = owner;
			_map = VariableMap.Get(owner.GetType());
		}

		public int VariableCount => _map.VariableCount;
		public Variable GetVariable(int index) => _map.GetVariable(_owner, index);
		public SetVariableResult SetVariable(int index, Variable value) => _map.SetVariable(_owner, index, value);

		public IReadOnlyList<string> VariableNames => _map.VariableNames;
		public Variable GetVariable(string name) => _map.GetVariable(_owner, name);
		public SetVariableResult SetVariable(string name, Variable value) => _map.SetVariable(_owner, name, value);
	}
}