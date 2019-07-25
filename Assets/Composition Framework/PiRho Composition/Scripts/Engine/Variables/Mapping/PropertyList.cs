namespace PiRhoSoft.Composition
{
	internal class PropertyList : IMappedVariableList
	{
		private object _owner;
		private PropertyMap _map;

		public PropertyList(object owner, PropertyMap map)
		{
			_owner = owner;
			_map = map;
		}

		#region IVariableList Implementation

		public int VariableCount => _map.Properties.Count;

		public string GetVariableName(int index)
		{
			return index >= 0 && index < _map.Properties.Count ? _map.Properties[index].Name : string.Empty;
		}

		public Variable GetVariableValue(int index)
		{
			if (index >= 0 && index < _map.Properties.Count) return _map.Properties[index].Get(_owner);
			else return Variable.Empty;
		}

		public SetVariableResult SetVariableValue(int index, Variable value)
		{
			if (index >= 0 && index < _map.Properties.Count) return _map.Properties[index].Set(_owner, value);
			else return SetVariableResult.NotFound;
		}

		#endregion
	}
}
