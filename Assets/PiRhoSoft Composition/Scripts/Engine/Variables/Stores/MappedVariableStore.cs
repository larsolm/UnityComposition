namespace PiRhoSoft.CompositionEngine
{
	public class MappedVariableStore : IVariableStore, IVariableList
	{
		private VariableMap _map;
		private IVariableList[] _lists;

		public void Setup(VariableMap map, params IVariableList[] lists)
		{
			_map = map;
			_lists = lists;
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			return _map != null && _map.TryGetIndex(name, out int index)
				? GetVariableValue(index)
				: VariableValue.Empty;
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			return _map != null && _map.TryGetIndex(name, out int index)
				? SetVariableValue(index, value)
				: SetVariableResult.NotFound;
		}

		#endregion

		#region IVariableList Implementation

		public int VariableCount
		{
			get { return _map != null ? _map.Count : 0; }
		}

		public string GetVariableName(int index)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.GetVariableName(index);

					index -= list.VariableCount;
				}
			}

			return null;
		}

		public VariableValue GetVariableValue(int index)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.GetVariableValue(index);

					index -= list.VariableCount;
				}
			}

			return VariableValue.Empty;
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.SetVariableValue(index, value);

					index -= list.VariableCount;
				}
			}

			return SetVariableResult.NotFound;
		}

		#endregion
	}
}
