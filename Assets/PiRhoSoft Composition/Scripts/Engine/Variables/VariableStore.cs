using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
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

		public void AddVariable(string name, VariableValue value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(Variable.Create(name, value));
		}

		public virtual VariableValue GetVariable(string name)
		{
			return _map.TryGetValue(name, out int index) ? _variables[index].Value : VariableValue.Empty;
		}

		public virtual SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (_map.TryGetValue(name, out int index))
				_variables[index] = Variable.Create(name, value);
			else
				AddVariable(name, value);

			return SetVariableResult.Success;
		}

		public IEnumerable<string> GetVariableNames()
		{
			return _map.Keys;
		}

		public void Clear()
		{
			_variables.Clear();
			_map.Clear();
		}
	}
}
