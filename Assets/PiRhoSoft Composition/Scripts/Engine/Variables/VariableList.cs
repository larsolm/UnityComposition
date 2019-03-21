using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public interface IVariableList
	{
		int Count { get; }
		VariableValue GetVariable(int index);
		SetVariableResult SetVariable(int index, VariableValue value);
		SetVariableResult AddVariable(VariableValue value);
		SetVariableResult RemoveVariable(int index);
	}

	public class VariableList : IVariableList
	{
		private List<VariableValue> _values = new List<VariableValue>();

		public int Count => _values.Count;

		public VariableValue GetVariable(int index)
		{
			if (index >= 0 && index < _values.Count)
				return _values[index];
			else
				return VariableValue.Empty;
		}

		public SetVariableResult AddVariable(VariableValue value)
		{
			_values.Add(value);
			return SetVariableResult.Success;
		}

		public SetVariableResult RemoveVariable(int index)
		{
			if (index >= 0 && index < _values.Count)
			{
				_values.RemoveAt(index);
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}

		public SetVariableResult SetVariable(int index, VariableValue value)
		{
			if (index >= 0 && index < _values.Count)
			{
				_values[index] = value;
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}
	}
}
