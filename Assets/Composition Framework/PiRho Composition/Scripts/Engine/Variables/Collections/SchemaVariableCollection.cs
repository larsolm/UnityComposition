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
		public VariableSchema Schema { get; private set; }
		public IVariableCollection Owner { get; private set; }
		private List<Variable> _variables = new List<Variable>();

		[SerializeField] private int _schemaVersion = 1;
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		public SchemaVariableCollection()
		{
		}

		public SchemaVariableCollection(VariableSchema schema)
		{
			Setup(schema, null);
		}

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
			if (Schema!= null && index >= 0 && index < _variables.Count)
				return UpdateVariable(index, value);
			else
				return SetVariableResult.NotFound;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => Schema?.Names ?? VariableStore.EmptyNames;
		}

		public Variable GetVariable(string name)
		{
			return Schema != null && Schema.TryGetIndex(name, out var index)
				? _variables[index]
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (Schema != null && Schema.TryGetIndex(name, out var index))
				return UpdateVariable(index, value);
			else
				return SetVariableResult.NotFound;
		}

		private SetVariableResult UpdateVariable(int index, Variable value)
		{
			var entry = Schema.GetEntry(index);

			if (entry.Definition.IsValid(value))
			{
				_variables[index] = value;
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		#region Persistence

		public void CopyTo(IVariableCollection variables, string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < _variables.Count; i++)
				{
					if (Schema.TryGetEntry(i, out var entry) && (string.IsNullOrEmpty(tag) || entry.Tag == tag))
						variables.SetVariable(entry.Definition.Name, _variables[i]);
				}
			}
		}

		public void CopyFrom(IVariableCollection variables, string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < _variables.Count; i++)
				{
					if (Schema.TryGetEntry(i, out var entry) && (string.IsNullOrEmpty(tag) || entry.Tag == tag))
					{
						var variable = variables.GetVariable(entry.Definition.Name);

						if (entry.Definition.IsValid(variable))
							_variables[i] = variable;
					}
				}
			}
		}

		#endregion

		#region Schema Management

		public bool NeedsUpdate => Schema != null && _schemaVersion != Schema.Version;

		public void Setup(VariableSchema schema, IVariableCollection owner)
		{
			Schema = schema;
			Owner = owner;

			Update();
		}

		public void Update()
		{
			if (NeedsUpdate)
			{
				var variables = new List<Variable>(Schema.EntryCount);

				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);
					var variable = GetVariable(entry.Definition.Name);

					variables.Add(variable);
				}

				_variables = variables;
				_schemaVersion = Schema.Version;

				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);

					// the list must be updated completely first before any initializers are run in case the schema has
					// any initializers that reference other variables on the same list.

					if (!entry.Definition.IsValid(_variables[i]))
						_variables[i] = entry.GenerateVariable(Owner);
				}
			}
		}

		private void ResetVariable(int index)
		{
			if (Schema != null && Schema.TryGetEntry(index, out var entry))
				_variables[index] = entry.GenerateVariable(Owner);
		}

		public void Clear()
		{
			_variables.Clear();
			_schemaVersion = 0;
			Schema = null;
			Owner = null;
		}

		#endregion

		#region Reset

		public void ResetAll()
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
					ResetVariable(i);
			}
		}

		public void ResetTag(string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);

					if (entry.Tag == tag)
						ResetVariable(i);
				}
			}
		}

		public void ResetVariables(IList<string> variables)
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);

					if (variables.Contains(entry.Definition.Name))
						ResetVariable(i);
				}
			}
		}

		#endregion

		#region Serialization

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_variablesData.SaveData(this, 1);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_variablesData.LoadData(this);
		}

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
	}
}