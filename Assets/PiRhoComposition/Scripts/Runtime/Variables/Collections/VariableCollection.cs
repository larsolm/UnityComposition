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

		#region Schema Handling

		public void ResetVariable(string variable, VariableSchema schema = null, IVariableMap lookup = null)
		{
			var value = GetVariable(variable);

			if (schema != null && schema.TryGetEntry(variable, out var entry))
				base.SetVariable(variable, entry.GenerateVariable(lookup));
			else
				base.SetVariable(variable, Variable.Create(value.Type));
		}

		public void ResetToSchema(VariableSchema schema, IVariableMap lookup)
		{
			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				base.RemoveVariable(entry.Definition.Name);
			}

			ApplySchema(schema, lookup);
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
						SetVariable(entry.Definition.Name, generated);
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
