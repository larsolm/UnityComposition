using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public enum SetVariableResult
	{
		Success,
		NotFound,
		ReadOnly,
		TypeMismatch
	}

	public interface IVariableStore
	{
		IList<string> GetVariableNames();
		Variable GetVariable(string name);
		SetVariableResult SetVariable(string name, Variable value);
	}

	public class VariableStore : IVariableStore
	{
		private List<string> _names = new List<string>();
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		public List<string> Names => _names;
		public List<Variable> Variables => _variables;
		public Dictionary<string, int> Map => _map;

		public virtual void AddVariable(string name, Variable value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(value);
			_names.Add(name);
		}

		public bool RemoveVariable(string name)
		{
			if (_map.TryGetValue(name, out var index))
			{
				RemoveVariable(name, index);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemoveVariable(int index)
		{
			var name = _names[index];
			RemoveVariable(name, index);
		}

		protected virtual void RemoveVariable(string name, int index)
		{
			_map.Remove(name);
			_variables.RemoveAt(index);
			_names.RemoveAt(index);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;
		}

		public virtual void VariableMoved(int from, int to)
		{
			_map.Clear();

			for (var i = 0; i < _names.Count; i++)
				_map.Add(_names[i], i);
		}

		public virtual void Clear()
		{
			_variables.Clear();
			_names.Clear();
			_map.Clear();
		}

		protected SetVariableResult SetVariable(string name, Variable value, bool allowAdd)
		{
			if (_map.TryGetValue(name, out int index))
				_variables[index] = value;
			else if (allowAdd)
				AddVariable(name, value);
			else
				return SetVariableResult.NotFound;

			return SetVariableResult.Success;
		}

		#region IVariableStore Implementation

		public virtual IList<string> GetVariableNames()
		{
			return _names;
		}

		public virtual Variable GetVariable(string name)
		{
			return _map.TryGetValue(name, out var index) ? _variables[index] : Variable.Empty;
		}

		public virtual SetVariableResult SetVariable(string name, Variable value)
		{
			return SetVariable(name, value, true);
		}

		#endregion
	}
}
