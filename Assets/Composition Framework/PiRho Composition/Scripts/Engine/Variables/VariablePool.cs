using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class VariablePool : VariableStore, ISerializableData, ISerializationCallbackReceiver
	{
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		public List<VariableDefinition> Definitions = new List<VariableDefinition>();

		public override void AddVariable(string name, Variable value)
		{
			Definitions.Add(new VariableDefinition(name, value.Type));
			base.AddVariable(name, value);
		}

		protected override void RemoveVariable(string name, int index)
		{
			Definitions.RemoveAt(index);
			base.RemoveVariable(name, index);
		}

		public override void Clear()
		{
			Definitions.Clear();
			base.Clear();
		}

		public override void VariableMoved(int from, int to)
		{
			var definition = Definitions[from];

			Definitions.RemoveAt(from);
			Definitions.Insert(to, definition);

			base.VariableMoved(from, to);
		}

		public void ChangeName(int index, string name)
		{
			var variableName = Names[index];

			if (variableName != name)
			{
				Map.Remove(variableName);
				Map.Add(name, index);
				
				Names[index] = name;
			}
		}

		public void ChangeDefinition(int index, VariableDefinition definition)
		{
			var variable = Variables[index];

			if (!definition.IsValid(variable))
				Variables[index] = definition.Generate();

			Definitions[index] = definition;
		}

		public SetVariableResult SetVariable(int index, Variable value)
		{
			var name = Names[index];
			var result = SetVariable(name, value);

			if (result == SetVariableResult.Success && value.Type != Definitions[index].Type)
				Definitions[index] = new VariableDefinition(name, value.Type);

			return result;
		}

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
			var count = Math.Min(Names.Count, Variables.Count);
			writer.Write(count);

			for (var i = 0; i < count; i++)
			{
				writer.Write(Names[i]);
				VariableHandler.Save(Variables[i], writer, data);
			}
		}

		public void Load(BinaryReader reader, SerializedData data)
		{
			base.Clear();
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var variable = VariableHandler.Load(reader, data);

				base.AddVariable(name, variable); // bypass the override so the definition isn't added
			}
		}

		#endregion
	}
}
