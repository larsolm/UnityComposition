using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class OpenStore : IVariableArray, IVariableCollection, IResettableVariables, ISerializableData, ISerializationCallbackReceiver
	{
		[SerializeField] private List<VariableDefinition> _definitions = new List<VariableDefinition>();
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();
		[SerializeField] private SerializedData _variablesData = new SerializedData();

		#region Access By Index

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

		#endregion

		#region Access By Name

		public IReadOnlyList<string> VariableNames
		{
			get => _definitions.Select(d => d.Name).ToList(); // TODO: cache?
		}

		public Variable GetVariable(string name)
		{
			if (_map.TryGetValue(name, out int index))
				return _variables[index];

			return Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_map.TryGetValue(name, out int index))
				return UpdateVariable(index, value);
			else
				return SetVariableResult.NotFound;
		}

		#endregion

		#region Modify

		public bool AddVariable(string name, Variable value)
		{
			if (!_map.TryGetValue(name, out _))
			{
				AddVariable(name, _variables.Count, value);
				return true;
			}

			return false;
		}

		public bool RemoveVariable(string name)
		{
			if (_map.TryGetValue(name, out var index))
			{
				RemoveVariable(name, index);
				return true;
			}

			return false;
		}

		public void Clear()
		{
			_definitions.Clear();
			_variables.Clear();
			_map.Clear();
		}

		private void AddVariable(string name, int index, Variable value)
		{
			_definitions.Add(new VariableDefinition(name, value.Type));
			_variables.Add(value);
			_map.Add(name, index);
		}

		private void RemoveVariable(string name, int index)
		{
			_definitions.RemoveAt(index);
			_variables.RemoveAt(index);
			_map.Remove(name);

			for (var i = index; i < _definitions.Count; i++)
				_map[_definitions[i].Name] = i;
		}

		private SetVariableResult UpdateVariable(int index, Variable value)
		{
			if (_definitions[index].IsValid(value))
			{
				_variables[index] = value;
				return SetVariableResult.Success;
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}

		#endregion

		#region Reset

		public void ResetTag(string tag)
		{
		}

		public void ResetVariables(IList<string> variables)
		{
			foreach (var name in variables)
			{
				if (_map.TryGetValue(name, out var index))
					ResetVariable(index);
			}
		}

		public void ResetAll()
		{
			for (var i = 0; i < Definitions.Count; i++)
				ResetVariable(i);
		}

		private void ResetVariable(int index)
		{
			_variables[index] = _definitions[index].Generate();
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
			Clear();
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var variable = VariableHandler.Load(reader, data);

				AddVariable(name, variable);
			}
		}

		#endregion

		#region Editor Support

		public List<VariableDefinition> Definitions => _definitions;
		public List<Variable> Variables => _variables;
		public Dictionary<string, int> Map => _map;

		public void ChangeName(int index, string name)
		{
			var oldName = _definitions[index].Name;
			_definitions[index].Name = name;
			_map.Remove(oldName);
			_map.Add(name, index);
		}

		public void ChangeType(int index, VariableType type)
		{
			_definitions[index].Type = type;
			_variables[index] = _definitions[index].Generate();
		}

		public void ChangeOrder(int from, int to)
		{
			var fromName = _definitions[from].Name;
			var toName = _definitions[to].Name;
			_map[fromName] = to;
			_map[toName] = from;

			var definition = _definitions[from];
			_definitions[from] = _definitions[to];
			_definitions[to] = definition;

			var variable = _variables[from];
			_variables[from] = _variables[to];
			_variables[to] = variable;
		}

		public void ChangeVariable(int index, Variable value)
		{
			_variables[index] = value;

			if (_definitions[index].Type != value.Type)
				_definitions[index].Type = value.Type;
		}

		#endregion
	}
}