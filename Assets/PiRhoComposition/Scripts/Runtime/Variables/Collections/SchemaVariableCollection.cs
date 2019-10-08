using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class SchemaVariableCollection : IVariableArray, IVariableCollection, IResettableVariables, ISerializableData, ISerializationCallbackReceiver
	{
		[SerializeField] private VariableSchema _schema = null;
		[SerializeField] private int _schemaVersion = 1;
		private List<Variable> _variables = new List<Variable>();
		[SerializeField] private List<string> _names = new List<string>();
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		public VariableSchema Schema => _schema;
		public IVariableCollection Owner { get; }

		public SchemaVariableCollection() => Owner = null;
		public SchemaVariableCollection(IVariableCollection owner) => Owner = owner;

		#region Schema Management

		public void SetSchema(VariableSchema schema)
		{
			if (schema == null)
				ClearSchema();
			else if (schema != _schema || schema.Version != _schemaVersion)
				UpdateSchema(schema);
		}

		private void ClearSchema()
		{
			_schema = null;
			_schemaVersion = 1;
			_variables.Clear();
			_names.Clear();
		}

		private void UpdateSchema(VariableSchema schema)
		{
			var variables = new List<Variable>(schema.EntryCount);
			var names = new List<string>(schema.EntryCount);

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				var index = _names.IndexOf(entry.Definition.Name);
				var variable = index >= 0 ? _variables[index] : Variable.Empty;

				variables.Add(variable);
				names.Add(entry.Definition.Name);
			}

			_schema = schema;
			_schemaVersion = schema.Version;
			_variables = variables;
			_names = names;

			for (var i = 0; i < _schema.EntryCount; i++)
			{
				var entry = _schema.GetEntry(i);

				// the list must be updated completely first before any initializers are run in case the schema has
				// any initializers that reference other variables on the same list.

				if (!entry.Definition.IsValid(_variables[i]))
					_variables[i] = entry.GenerateVariable(Owner);
			}
		}

		private SetVariableResult UpdateVariable(int index, Variable value)
		{
			var entry = _schema.GetEntry(index);

			if (entry.Definition.IsValid(value))
			{
				_variables[index] = value;
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

		#region Persistence

		public void CopyTo(IVariableCollection variables, string tag)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _variables.Count; i++)
				{
					if (_schema.TryGetEntry(i, out var entry) && (string.IsNullOrEmpty(tag) || entry.Tag == tag))
						variables.SetVariable(entry.Definition.Name, _variables[i]);
				}
			}
		}

		public void CopyFrom(IVariableCollection variables, string tag)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _variables.Count; i++)
				{
					if (_schema.TryGetEntry(i, out var entry) && (string.IsNullOrEmpty(tag) || entry.Tag == tag))
					{
						var variable = variables.GetVariable(entry.Definition.Name);

						if (entry.Definition.IsValid(variable))
							_variables[i] = variable;
					}
				}
			}
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

		public SetVariableResult SetVariable(int index, Variable value)
		{
			if (_schema != null && index >= 0 && index < _variables.Count)
				return UpdateVariable(index, value);
			else
				return SetVariableResult.NotFound;
		}

		#endregion

		#region IVariableCollection Implementation

		public IReadOnlyList<string> VariableNames
		{
			get => _schema?.Names ?? VariableStore.EmptyNames;
		}

		public Variable GetVariable(string name)
		{
			return _schema != null && _schema.TryGetIndex(name, out var index)
				? _variables[index]
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_schema != null && _schema.TryGetIndex(name, out var index))
				return UpdateVariable(index, value);
			else
				return SetVariableResult.NotFound;
		}

		#endregion

		#region IResettableVariables Implementation

		public void ResetAll()
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.EntryCount; i++)
				{
					var entry = _schema.GetEntry(i);
					_variables[i] = entry.GenerateVariable(Owner);
				}
			}
		}

		public void ResetTag(string tag)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.EntryCount; i++)
				{
					var entry = _schema.GetEntry(i);

					if (entry.Tag == tag)
						_variables[i] = entry.GenerateVariable(Owner);
				}
			}
		}

		public void ResetVariables(IList<string> variables)
		{
			if (_schema != null)
			{
				for (var i = 0; i < _schema.EntryCount; i++)
				{
					var entry = _schema.GetEntry(i);

					if (variables.Contains(entry.Definition.Name))
						_variables[i] = entry.GenerateVariable(Owner);
				}
			}
		}

		#endregion

		#region ISerializableData Implementation

		void ISerializableData.Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write(_variables.Count);

			for (var i = 0; i < _variables.Count; i++)
				VariableHandler.Save(_variables[i], writer, data);
		}

		void ISerializableData.Load(BinaryReader reader, SerializedData data)
		{
			var count = reader.ReadInt32();

			_variables.Clear();
			_variables.Capacity = count;

			for (var i = 0; i < count; i++)
			{
				var value = VariableHandler.Load(reader, data);
				_variables.Add(value);
			}
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_variablesData.SaveData(this, 1);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_variablesData.LoadData(this);
			SetSchema(_schema);
		}

		#endregion
	}
}
