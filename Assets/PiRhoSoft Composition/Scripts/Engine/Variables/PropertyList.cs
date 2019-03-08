namespace PiRhoSoft.CompositionEngine
{
	public class PropertyList<OwnerType> : IVariableList where OwnerType : class
	{
		private OwnerType _owner;
		private PropertyMap<OwnerType> _map;

		public PropertyList(OwnerType owner, PropertyMap<OwnerType> map)
		{
			_owner = owner;
			_map = map;
		}

		#region IVariableList Implementation

		public int VariableCount => _map.Properties.Count;

		public string GetVariableName(int index)
		{
			return index >= 0 && index < _map.Properties.Count ? _map.Properties[index].Name : "";
		}

		public VariableValue GetVariableValue(int index)
		{
			var getter = index >= 0 && index < _map.Properties.Count ? _map.Properties[index].Getter : null;
			return getter != null ? getter(_owner) : VariableValue.Empty;
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			if (index < 0 || index >= _map.Properties.Count)
				return SetVariableResult.NotFound;

			var setter = _map.Properties[index].Setter;

			if (setter == null)
				return SetVariableResult.ReadOnly;

			return setter(_owner, value);
		}

		#endregion
	}
}
