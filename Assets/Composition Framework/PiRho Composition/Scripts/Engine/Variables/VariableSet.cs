using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public interface IVariableReset
	{
		void ResetTag(string tag);
		void ResetVariables(IList<string> variables);
	}

	public struct NamedVariable
	{
		public string Name { get; private set; }
		public Variable Variable { get; private set; }

		public static NamedVariable Empty => Create(string.Empty, Variable.Empty);

		public static NamedVariable Create(string name, Variable value)
		{
			return new NamedVariable
			{
				Name = name,
				Variable = value
			};
		}
	}
	[Serializable]
	public class VariableSet : IMappedVariableList, IVariableReset, ISerializableData, ISerializationCallbackReceiver
	{
		[NonSerialized] private VariableSchema _schema;
		[NonSerialized] private IVariableStore _owner;
		[NonSerialized] private List<NamedVariable> _variables = new List<NamedVariable>();

		[SerializeField] private int _schemaVersion = 1;
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		public VariableSchema Schema => _schema;
		public IVariableStore Owner => _owner;

		#region Persistence

		public void LoadFrom(VariableSet variables, string tag)
		{
			foreach (NamedVariable fromVariable in variables._variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (tag == null || _schema[index].Tag == tag)
						SetValue(index, fromVariable.Variable);
				}
			}
		}

		public void SaveTo(VariableSet variables, string tag)
		{
			foreach (NamedVariable fromVariable in _variables)
			{
				var index = _schema != null ? _schema.GetIndex(fromVariable.Name) : -1;

				if (index >= 0)
				{
					if (tag == null || _schema[index].Tag == tag)
						variables._variables.Add(fromVariable);
				}
			}

			variables._schemaVersion = _schemaVersion;
		}

		#endregion

		#region Schema Management

		public bool NeedsUpdate => _schema != null && _schemaVersion != _schema.Version;

		public void Setup(VariableSchema schema, IVariableStore owner)
		{
			_schema = schema;
			_owner = owner;

			Update();
		}

		public void Update()
		{
			if (_schema != null && _schemaVersion != _schema.Version)
			{
				var variables = new List<NamedVariable>(_schema.Count);

				for (var i = 0; i < _schema.Count; i++)
				{
					var definition = _schema[i].Definition;
					var variable = GetVariable(definition.Name);

					if (variable.Variable.Type == definition.Type)
						variables.Add(variable);
					else
						variables.Add(NamedVariable.Create(definition.Name, Variable.Empty));
				}

				_variables = variables;
				_schemaVersion = _schema.Version;

				for (var i = 0; i < _schema.Count; i++)
				{
					// The list must be updated completely first before any initializers are run in case the schema has
					// any initializers that reference other variables on the same list.

					if (_variables[i].Variable.Type == VariableType.Empty)
					{
						var definition = _schema[i].Definition;
						var value = Schema[i].GenerateValue(_owner);
						_variables[i] = NamedVariable.Create(definition.Name, value);
					}
				}
			}
		}

		public void Reset(int index)
		{
			if (_schema != null)
			{
				var definition = _schema[index].Definition;
				var value = _schema[index].GenerateValue(_owner);
				_variables[index] = NamedVariable.Create(definition.Name, value);
			}
		}

		public void Clear()
		{
			_variables.Clear();
			_schemaVersion = 0;
			_schema = null;
			_owner = null;
		}

		private NamedVariable GetVariable(string name)
		{
			for (var i = 0; i < _variables.Count; i++)
			{
				if (_variables[i].Name == name)
					return _variables[i];
			}

			return NamedVariable.Create(name, Variable.Empty);
		}

		private bool SetValue(int index, Variable value)
		{
			var existing = _variables[index].Variable;

			if (_schema != null)
			{
				var definition = _schema[index].Definition;

				if (definition.Type != VariableType.Empty && definition.Type != value.Type)
					return false;

				if (definition != null && !definition.IsValid(value))
					return false;
			}

			_variables[index] = NamedVariable.Create(_variables[index].Name, value);
			return true;
		}

		#endregion

		#region IMappedVariableList Implementation

		public int VariableCount
		{
			get { return _variables.Count; }
		}

		public string GetVariableName(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Name : null;
		}

		public Variable GetVariableValue(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Variable : Variable.Empty;
		}

		public SetVariableResult SetVariableValue(int index, Variable value)
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
					if (_schema[i].Tag == tag)
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
					if (variables.Contains(_schema[i].Definition.Name))
						Reset(i);
				}
			}
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			_variablesData.SaveInstance(this, 1);
		}

		public void OnAfterDeserialize()
		{
			_variablesData.LoadInstance(this);
		}

		public void Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write(_variables.Count);

			for (var i = 0; i < _variables.Count; i++)
			{
				writer.Write(_variables[i].Name);
				VariableHandler.Save(_variables[i].Variable, writer, data);
			}
		}

		public void Load(BinaryReader reader, SerializedData data)
		{
			var count = reader.ReadInt32();

			_variables.Clear();
			_variables.Capacity = count;

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var value = VariableHandler.Load(reader, data);

				_variables.Add(NamedVariable.Create(name, value));
			}
		}

		#endregion
	}
}
