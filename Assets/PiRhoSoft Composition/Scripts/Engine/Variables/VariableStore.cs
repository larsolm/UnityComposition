using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
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
		VariableValue GetVariable(string name);
		SetVariableResult SetVariable(string name, VariableValue value);
		IEnumerable<string> GetVariableNames();
	}

	public class VariableStore : IVariableStore
	{
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		public List<Variable> Variables => _variables;
		public Dictionary<string, int> Map => _map;

		public virtual void AddVariable(string name, VariableValue value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(Variable.Create(name, value));
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
			var name = _variables[index].Name;
			RemoveVariable(name, index);
		}

		protected virtual void RemoveVariable(string name, int index)
		{
			_map.Remove(name);
			_variables.RemoveAt(index);

			for (var i = index; i < _variables.Count; i++)
				_map[_variables[i].Name] = i;
		}

		public virtual void VariableMoved(int from, int to)
		{
			_map.Clear();

			for (var i = 0; i < _variables.Count; i++)
				_map.Add(_variables[i].Name, i);
		}

		public virtual void Clear()
		{
			_variables.Clear();
			_map.Clear();
		}

		protected SetVariableResult SetVariable(string name, VariableValue value, bool allowAdd)
		{
			if (_map.TryGetValue(name, out int index))
				_variables[index] = Variable.Create(name, value);
			else if (allowAdd)
				AddVariable(name, value);
			else
				return SetVariableResult.NotFound;

			return SetVariableResult.Success;
		}

		#region IVariableStore Implementation

		public virtual IEnumerable<string> GetVariableNames()
		{
			return _map.Keys;
		}

		public virtual VariableValue GetVariable(string name)
		{
			return _map.TryGetValue(name, out var index) ? _variables[index].Value : VariableValue.Empty;
		}

		public virtual SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariable(name, value, true);
		}

		#endregion
	}
}
