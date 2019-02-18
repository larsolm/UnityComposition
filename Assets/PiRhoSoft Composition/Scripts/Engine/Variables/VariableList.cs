using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum SetVariableResult
	{
		Success,
		NotFound,
		ReadOnly,
		TypeMismatch
	}

	public interface IVariableList
	{
		int VariableCount { get; }
		string GetVariableName(int index);
		VariableValue GetVariableValue(int index);
		SetVariableResult SetVariableValue(int index, VariableValue value);
	}

	[Serializable]
	public class VariableList : IVariableList, ISerializationCallbackReceiver
	{
		[SerializeField] private List<SerializedVariable> _data;
		[SerializeField] private int _version = 0;
		[NonSerialized] private VariableSchema _schema;
		[NonSerialized] private IVariableStore _owner;
		[NonSerialized] private List<Variable> _variables = new List<Variable>();

		public VariableSchema Schema => _schema;
		public IVariableStore Owner => _owner;

		#region Persistence

		public void LoadFrom(VariableList variables, string availability)
		{
			foreach (Variable fromVariable in variables._variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (availability == null || _schema[index].Availability == availability)
						SetValue(index, fromVariable.Value);
				}
			}
		}

		public void SaveTo(VariableList variables, string availability)
		{
			foreach (Variable fromVariable in _variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (availability == null || _schema[index].Availability == availability)
						variables._variables.Add(fromVariable);
				}
			}

			variables._version = _version;
		}

		#endregion

		#region Schema Management

		public bool NeedsUpdate => _schema != null && _version != _schema.Version;

		public void Setup(VariableSchema schema, IVariableStore owner)
		{
			_schema = schema;
			_owner = owner;

			Update();
		}

		public void Update()
		{
			if (_schema != null && _version != _schema.Version)
			{
				var variables = new List<Variable>(_schema.Count);

				for (var i = 0; i < _schema.Count; i++)
				{
					var definition = _schema[i];
					var variable = GetVariable(definition.Name);

					if (variable.Value.Type == definition.Type)
						variables.Add(variable);
					else
						variables.Add(Variable.Create(definition.Name, VariableValue.Empty));
				}

				_variables = variables;
				_version = _schema.Version;

				for (var i = 0; i < _schema.Count; i++)
				{
					// The list must be updated completely first before any initializers are run in case the schema has
					// any initializers that reference other variables on the same list.

					if (_variables[i].Value.Type == VariableType.Empty)
					{
						var definition = _schema[i];
						_variables[i] = definition.Generate(_owner);
					}
				}
			}
		}

		public void Reset(int index)
		{
			if (_schema != null)
				_variables[index] = _schema[index].Generate(_owner);
		}

		public void Reset(string availability)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.Count; i++)
				{
					if (_schema[i].Availability == availability)
						Reset(i);
				}
			}
		}

		public void Reset(IList<string> variables)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.Count; i++)
				{
					if (variables.Contains(_schema[i].Name))
						Reset(i);
				}
			}
		}

		public void Clear()
		{
			_variables.Clear();
			_version = 0;
			_schema = null;
			_owner = null;
		}

		private Variable GetVariable(string name)
		{
			for (var i = 0; i < _variables.Count; i++)
			{
				if (_variables[i].Name == name)
					return _variables[i];
			}

			return Variable.Create(name, VariableValue.Empty);
		}

		private bool SetValue(int index, VariableValue value)
		{
			if (_variables[index].Value.Type == value.Type || _variables[index].Value.Type == VariableType.Empty)
			{
				_variables[index] = Variable.Create(_variables[index].Name, value);
				return true;
			}

			return false;
		}

		#endregion

		#region IVariableList Implementation

		public int VariableCount
		{
			get { return _variables.Count; }
		}

		public string GetVariableName(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Name : null;
		}

		public VariableValue GetVariableValue(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Value : VariableValue.Empty;
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			if (index >= 0 && index < _variables.Count)
				return SetValue(index, value) ? SetVariableResult.Success : SetVariableResult.TypeMismatch;
			else
				return SetVariableResult.NotFound;
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			_data = new List<SerializedVariable>(_variables.Count);

			var builder = new StringBuilder();

			foreach (var variable in _variables)
			{
				var data = new SerializedVariable();
				data.SetVariable(variable);
				_data.Add(data);
			}
		}

		public void OnAfterDeserialize()
		{
			_variables.Clear();

			if (_data != null)
			{
				foreach (var data in _data)
				{
					var variable = data.GetVariable();
					_variables.Add(variable);
				}
			}

			_data = null;
		}

		#endregion
	}
}
