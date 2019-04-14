using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public interface IVariableReset
	{
		void ResetTag(string tag);
		void ResetVariables(IList<string> variables);
	}

	[Serializable]
	public class VariableSet : IMappedVariableList, IVariableReset, ISerializationCallbackReceiver
	{
		[NonSerialized] private VariableSchema _schema;
		[NonSerialized] private IVariableStore _owner;
		[NonSerialized] private List<Variable> _variables = new List<Variable>();

		[SerializeField] private int _version = 0;
		[SerializeField] private List<string> _variablesData;
		[SerializeField] private List<Object> _variablesObjects;

		public VariableSchema Schema => _schema;
		public IVariableStore Owner => _owner;

		#region Persistence

		public void LoadFrom(VariableSet variables, string tag)
		{
			foreach (Variable fromVariable in variables._variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (tag == null || _schema[index].Definition.Tag == tag)
						SetValue(index, fromVariable.Value);
				}
			}
		}

		public void SaveTo(VariableSet variables, string tag)
		{
			foreach (Variable fromVariable in _variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (tag == null || _schema[index].Definition.Tag == tag)
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

					if (variable.Value.Type == definition.Definition.Type)
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
						var value = definition.Definition.Generate(_owner);
						_variables[i] = Variable.Create(definition.Name, value);
					}
				}
			}
		}

		public void Reset(int index)
		{
			if (_schema != null)
			{
				var definition = _schema[index];
				var value = definition.Definition.Generate(_owner);
				_variables[index] = Variable.Create(definition.Name, value);
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
			var existing = _variables[index].Value;

			if (_schema != null)
			{
				var definition = _schema[index].Definition;

				if (definition.Type != VariableType.Empty && definition.Type != value.Type)
					return false;

				if (definition.Constraint != null && !definition.Constraint.IsValid(value))
					return false;
			}

			_variables[index] = Variable.Create(_variables[index].Name, value);
			return true;
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

		#region IVariableReset Implementation

		public void ResetTag(string tag)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.Count; i++)
				{
					if (_schema[i].Definition.Tag == tag)
						Reset(i);
				}
			}
		}

		public void ResetVariables(IList<string> variables)
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

		#endregion

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			_variablesData = VariableHandler.SaveVariables(_variables, ref _variablesObjects);
		}

		public void OnAfterDeserialize()
		{
			_variables = VariableHandler.LoadVariables(ref _variablesData, ref _variablesObjects);
		}

		#endregion
	}
}
