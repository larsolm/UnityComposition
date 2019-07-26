using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IVariableIndex
	{
		int VariableCount { get; }
		Variable GetVariable(int index);
		SetVariableResult SetVariable(int index, Variable value);
	}

	public interface IVariableMap
	{
		IReadOnlyList<string> VariableNames { get; }
		Variable GetVariable(string name);
		SetVariableResult SetVariable(string name, Variable value);
	}

	public interface IVariableReset
	{
		void ResetAll();
		void ResetTag(string tag);
		void ResetVariables(IList<string> variables);
	}

	public class VariableStore : IVariableIndex, IVariableMap
	{
		private List<string> _names = new List<string>();
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		public int VariableCount
		{
			get => _variables.Count;
		}

		public Variable GetVariable(int index)
		{
			return index >= 0 && index < _variables.Count
				? _variables[index]
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(int index, Variable value)
		{
			if (value.IsEmpty)
				RemoveVariable(index);
			else if (index >= 0 && index < _variables.Count)
				_variables[index] = value;
			else
				return SetVariableResult.NotFound;

			return SetVariableResult.Success;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => _names;
		}

		public Variable GetVariable(string name)
		{
			return _map.TryGetValue(name, out var index)
				? _variables[index]
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_map.TryGetValue(name, out int index))
			{
				if (value.IsEmpty)
					RemoveVariable(index);
				else
					_variables[index] = value;
			}
			else
			{
				AddVariable(name, value);
			}

			return SetVariableResult.Success;
		}

		private void AddVariable(string name, Variable value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(value);
			_names.Add(name);
		}

		private void RemoveVariable(int index)
		{
			_map.Remove(_names[index]);
			_variables.RemoveAt(index);
			_names.RemoveAt(index);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;
		}
	}
}