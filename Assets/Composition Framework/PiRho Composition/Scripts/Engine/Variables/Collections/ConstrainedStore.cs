using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class ConstrainedStore : IVariableArray, IVariableCollection, IResettableVariables, ISerializableData, ISerializationCallbackReceiver
	{
		public VariableSchema Schema { get; private set; }
		public IVariableCollection Owner { get; private set; }
		private List<Variable> _variables = new List<Variable>();

		[SerializeField] private int _schemaVersion = 1;
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		public ConstrainedStore()
		{
		}

		public ConstrainedStore(VariableSchema schema)
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
			if (index >= 0 && index < _variables.Count)
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
			if (Schema.IsValid(index, value))
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
					if (Schema.GetTag(i) == tag)
					{
						var name = Schema.GetName(i);
						variables.SetVariable(name, _variables[i]);
					}
				}
			}
		}

		public void CopyFrom(IVariableCollection variables, string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < _variables.Count; i++)
				{
					if (Schema.GetTag(i) == tag)
					{
						var name = Schema.GetName(i);
						var variable = variables.GetVariable(name);

						if (Schema.IsValid(i, variable))
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
				var variables = new List<Variable>(Schema.Count);

				for (var i = 0; i < Schema.Count; i++)
				{
					var name = Schema.GetName(i);
					var variable = GetVariable(name);

					variables.Add(variable);
				}

				_variables = variables;
				_schemaVersion = Schema.Version;

				for (var i = 0; i < Schema.Count; i++)
				{
					// the list must be updated completely first before any initializers are run in case the schema has
					// any initializers that reference other variables on the same list.

					if (!Schema.IsValid(i, _variables[i]))
						_variables[i] = Schema.Generate(Owner, i);
				}
			}
		}

		private void ResetVariable(int index)
		{
			if (Schema != null)
				_variables[index] = Schema.Generate(Owner, index);
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
				for (var i = 0; i < Schema.Count; i++)
					ResetVariable(i);
			}
		}

		public void ResetTag(string tag)
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.Count; i++)
				{
					if (Schema.GetTag(i) == tag)
						ResetVariable(i);
				}
			}
		}

		public void ResetVariables(IList<string> variables)
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.Count; i++)
				{
					if (variables.Contains(Schema.GetName(i)))
						ResetVariable(i);
				}
			}
		}

		#endregion

		#region Serialization

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_variablesData.SaveInstance(this, 1);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_variablesData.LoadInstance(this);
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