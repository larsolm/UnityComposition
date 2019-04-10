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

		public void AddVariable(string name, VariableValue value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(Variable.Create(name, value));
		}

		public bool RemoveVariable(string name)
		{
			return _map.Remove(name);
		}

		public virtual VariableValue GetVariable(string name)
		{
			return _map.TryGetValue(name, out int index) ? _variables[index].Value : VariableValue.Empty;
		}

		public virtual SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariable(name, value, true);
		}

		public virtual IEnumerable<string> GetVariableNames()
		{
			return _map.Keys;
		}

		public void Clear()
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
	}

	public class WritableStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariable(name, value, false);
		}
	}

	public class ReadOnlyStore : VariableStore
	{
		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (Map.TryGetValue(name, out int index))
				return SetVariableResult.ReadOnly;
			else
				return SetVariableResult.NotFound;
		}
	}

	public class ConstrainedStore : WritableStore, ISchemaOwner
	{
		public VariableSchema Schema { get; private set; }

		public ConstrainedStore(VariableSchema schema)
		{
			Schema = schema;
			SetupSchema();
		}

		public void SetupSchema()
		{
			for (var i = 0; i < Schema.Count; i++)
			{
				var definition = Schema[i];
				var value = definition.Definition.Generate(null);

				AddVariable(definition.Name, value);
			}
		}
	}
}
