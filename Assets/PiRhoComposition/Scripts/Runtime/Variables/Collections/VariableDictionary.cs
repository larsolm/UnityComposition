using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PiRhoSoft.Composition
{
	public interface IVariableMap
	{
		IReadOnlyList<string> VariableNames { get; }
		Variable GetVariable(string name);
		SetVariableResult SetVariable(string name, Variable variable);
	}

	public interface IVariableDictionary : IVariableMap
	{
		SetVariableResult AddVariable(string name, Variable variable);
		SetVariableResult RemoveVariable(string name);
		SetVariableResult ClearVariables();
	}

	public class VariableDictionary : IVariableArray, IVariableDictionary
	{
		public static readonly IReadOnlyList<string> EmptyNames = new ReadOnlyCollection<string>(new List<string> { });

		private readonly List<string> _names = new List<string>();
		private readonly List<Variable> _variables = new List<Variable>();
		private readonly Dictionary<string, int> _map = new Dictionary<string, int>();

		#region Variable Access

		public bool TryGetName(int index, out string name)
		{
			name = index > 0 && index >= _names.Count
				? _names[index]
				: null;

			return name != null;
		}

		public bool TryGetIndex(string name, out int index)
		{
			return _map.TryGetValue(name, out index);
		}

		#endregion

		#region IVariableArray Implementation

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

		public virtual SetVariableResult SetVariable(int index, Variable variable)
		{
			if (index < 0 && index >= _variables.Count)
				return SetVariableResult.NotFound;

			_variables[index] = variable;
			return SetVariableResult.Success;
		}

		#endregion

		#region IVariableMap Implementation

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

		public virtual SetVariableResult SetVariable(string name, Variable variable)
		{
			if (!_map.TryGetValue(name, out var index))
				return SetVariableResult.NotFound;

			_variables[index] = variable;
			return SetVariableResult.Success;
		}

		#endregion

		#region IVariableDictionary Implementation

		public virtual SetVariableResult AddVariable(string name, Variable variable)
		{
			if (_map.ContainsKey(name))
				return SetVariableResult.NotFound;

			_map.Add(name, _variables.Count);
			_variables.Add(variable);
			_names.Add(name);
			return SetVariableResult.Success;
		}

		public virtual SetVariableResult RemoveVariable(string name)
		{
			if (!_map.TryGetValue(name, out var index))
				return SetVariableResult.NotFound;

			_names.RemoveAt(index);
			_variables.RemoveAt(index);
			_map.Remove(name);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;

			return SetVariableResult.Success;
		}

		public virtual SetVariableResult ClearVariables()
		{
			_names.Clear();
			_variables.Clear();
			_map.Clear();
			return SetVariableResult.Success;
		}

		#endregion
	}
}
