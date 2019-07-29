using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public struct ObjectVariableCollection
	{
		private object _owner;
		private IVariableCollection _collection;
		private VariableMap _map;

		public ObjectVariableCollection(object owner)
		{
			_owner = owner;
			_collection = owner as IVariableCollection;
			_map = _collection == null && _owner != null ? VariableMap.Get(_owner.GetType()) : null;

		}

		public bool IsValid => _collection != null || _map != null;

		public IReadOnlyList<string> VariableNames
		{
			get => _collection?.VariableNames ?? _map?.VariableNames;
		}

		public Variable GetVariable(string name)
		{
			if (_collection != null)
				return _collection.GetVariable(name);
			else if (_map != null)
				return _map.GetVariable(_owner, name);
			else
				return Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_collection != null)
				return _collection.SetVariable(name, value);
			else if (_map != null)
				return _map.SetVariable(_owner, name, value);
			else
				return SetVariableResult.NotFound;
		}
	}
}