using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class VariableCollection : VariableDictionary, ISerializationCallbackReceiver
	{
		public const string DataProperty = nameof(_data);

		public SerializedDataList Data => _data;
		[SerializeField] private SerializedDataList _data = new SerializedDataList();

		public VariableSchema Schema;
		public List<VariableDefinition> Definitions = new List<VariableDefinition>();
		
		#region Schema Handling

		public void ResetVariable(string variable, VariableSchema schema, IVariableMap lookup = null)
		{
			if (schema.TryGetEntry(variable, out var entry))
				base.SetVariable(variable, entry.GenerateVariable(lookup ?? this));
		}

		public void ResetToSchema(VariableSchema schema, IVariableMap lookup = null)
		{
			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				base.SetVariable(entry.Definition.Name, entry.GenerateVariable(lookup ?? this));
			}
		}

		public void ApplySchema(VariableSchema schema, IVariableMap lookup)
		{
			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);

				if (TryGetIndex(entry.Definition.Name, out var index))
				{
					var variable = GetVariable(index);

					if (!entry.Definition.IsValid(variable))
					{
						var generated = entry.GenerateVariable(lookup);
						SetVariable(index, generated);
					}
				}
				else
				{
					var generated = entry.GenerateVariable(lookup);
					AddVariable(entry.Definition.Name, generated);
				}
			}
		}

		public bool ImplementsSchema(VariableSchema schema, bool exact)
		{
			if (exact && schema.EntryCount != VariableCount)
				return false;

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);

				if (!TryGetIndex(entry.Definition.Name, out var index))
					return false;
				else if (!entry.Definition.IsValid(GetVariable(index)))
					return false;
			}

			return true;
		}

		#endregion

		#region Serialization

		protected virtual void Save()
		{
			_data.Clear();

			for (var i = 0; i < VariableCount; i++)
			{
				using (var writer = new SerializedDataWriter(_data))
				{
					writer.Writer.Write(VariableNames[i]);
					VariableHandler.Save(GetVariable(i), writer);
				}
			}
		}

		protected virtual void Load()
		{
			base.ClearVariables(); // Call to the base so a save isn't initiated;

			for (var i = 0; i < _data.Count; i++)
			{
				using (var reader = new SerializedDataReader(_data, i))
				{
					var name = reader.Reader.ReadString();
					var variable = VariableHandler.Load(reader);
					AddVariable(name, variable);
				}
			}
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		void ISerializationCallbackReceiver.OnBeforeSerialize() { }
		void ISerializationCallbackReceiver.OnAfterDeserialize() => Load();

		#endregion

		#region Editor Overrides

#if UNITY_EDITOR
		public VariableDefinition GetDefinition(string name)
		{
			if (Schema != null)
			{
				var entry = Schema.GetEntry(name);
				if (entry != null)
					return entry.Definition;
			}

			foreach (var definition in Definitions)
			{
				if (definition.Name == name)
					return definition;
			}

			return null;
		}

		public override SetVariableResult AddVariable(string name, Variable variable)
		{
			var result = base.AddVariable(name, variable);
			Save();
			return result;
		}

		public override SetVariableResult ClearVariables()
		{
			var result = base.ClearVariables();
			Save();
			return result;
		}

		public override SetVariableResult RemoveVariable(string name)
		{
			var result = base.RemoveVariable(name);
			Save();
			return result;
		}

		public override SetVariableResult SetVariable(int index, Variable variable)
		{
			var result = base.SetVariable(index, variable);
			Save();
			return result;
		}

		public override SetVariableResult SetVariable(string name, Variable variable)
		{
			var result = base.SetVariable(name, variable);
			Save();
			return result;
		}
#endif

		#endregion
	}
}
