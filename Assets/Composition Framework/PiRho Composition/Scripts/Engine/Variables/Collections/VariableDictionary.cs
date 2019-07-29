using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IVariableDictionary : IVariableCollection
	{
		VariableSchema Schema { get; }
	}

	public class VariableDictionary : IVariableDictionary
	{
		private List<string> _names = new List<string>();
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		public List<string> Names => _names;
		public List<Variable> Variables => _variables;
		public Dictionary<string, int> Map => _map;

		public VariableSchema Schema { get; private set; }

		public VariableDictionary()
		{
		}

		public VariableDictionary(VariableSchema schema)
		{
			Schema = schema;
			Reset();
		}

		public void AddVariable(string name, Variable value)
		{
			if (Schema == null)
				AddVariable(name, _variables.Count, value);
		}

		public bool RemoveVariable(string name)
		{
			if (Schema == null && _map.TryGetValue(name, out var index))
			{
				RemoveVariable(name, index);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public IReadOnlyList<string> VariableNames
		{
			get => _names;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_map.TryGetValue(name, out int index))
				_variables[index] = value;
			else if (Schema == null)
				AddVariable(name, _variables.Count, value);
			else
				return SetVariableResult.NotFound;

			return SetVariableResult.Success;
		}

		public Variable GetVariable(string name)
		{
			return _map.TryGetValue(name, out var index) ? _variables[index] : Variable.Empty;
		}

		public void Reset()
		{
			_variables.Clear();
			_names.Clear();
			_map.Clear();

			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);
					var value = entry.GenerateVariable(null);

					AddVariable(entry.Definition.Name, i, value);
				}
			}
		}

		private void AddVariable(string name, int index, Variable value)
		{
			_names.Add(name);
			_variables.Add(value);
			_map.Add(name, index);
		}

		private void RemoveVariable(string name, int index)
		{
			_names.RemoveAt(index);
			_variables.RemoveAt(index);
			_map.Remove(name);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;
		}
	}
}