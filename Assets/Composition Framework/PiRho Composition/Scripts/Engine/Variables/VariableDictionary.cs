﻿using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IVariableDictionary : IVariableStore, IVariableReset
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
			ResetAll();
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
		
		public IList<string> GetVariableNames()
		{
			return _names;
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

		public void ResetTag(string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.Count; i++)
				{
					if (Schema[i].Tag == tag)
						ResetVariable(i);
				}
			}
		}

		public void ResetVariables(IList<string> variables)
		{
			foreach (var name in variables)
			{
				if (_map.TryGetValue(name, out var index))
					ResetVariable(index);
			}
		}

		public void ResetAll()
		{
			_variables.Clear();
			_names.Clear();
			_map.Clear();

			if (Schema != null)
			{
				for (var i = 0; i < Schema.Count; i++)
				{
					var definition = Schema[i].Definition;
					var value = definition.Generate();

					AddVariable(definition.Name, i, value);
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

		private void ResetVariable(int index)
		{
			if (Schema != null)
			{
				var definition = Schema[index].Definition;
				_variables[index] = definition.Generate();
			}
			else
			{
				var type = _variables[index].Type;
				_variables[index] = Variable.Create(type);
			}
		}
	}
}