using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public interface IVariableCollection
	{
		IReadOnlyList<string> VariableNames { get; }
		Variable GetVariable(string name);
		SetVariableResult SetVariable(string name, Variable value);
	}

	public interface IVariableArray
	{
		int VariableCount { get; }
		Variable GetVariable(int index);
		SetVariableResult SetVariable(int index, Variable value);
	}

	public interface IResettableVariables
	{
		void ResetAll();
		void ResetTag(string tag);
		void ResetVariables(IList<string> variables);
	}

	[Serializable]
	public class VariableStore : IVariableArray, IVariableCollection, ISerializableData, ISerializationCallbackReceiver
	{
		public static IReadOnlyList<string> EmptyNames = new ReadOnlyCollection<string>(new List<string> { });

		[SerializeField] private List<string> _names = new List<string>();
		private List<Variable> _variables = new List<Variable>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();
		[SerializeField] private SerializedData _variablesData = new SerializedData();
		public bool Locked { get; set; }

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
			if (value.IsEmpty)
				RemoveVariable(index);
			else if (index >= 0 && index < _variables.Count)
				_variables[index] = value;
			else
				return SetVariableResult.NotFound;

			return SetVariableResult.Success;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => _names;
		}

		public Variable GetVariable(string name)
		{
			return _map.TryGetValue(name, out var index)
				? _variables[index]
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (_map.TryGetValue(name, out int index))
			{
				if (value.IsEmpty && !Locked)
					RemoveVariable(index);
				else
					_variables[index] = value;
			}
			else
			{
				if (Locked)
					return SetVariableResult.NotFound;
				else
					AddVariable(name, value);
			}

			return SetVariableResult.Success;
		}

		public void Clear()
		{
			_map.Clear();
			_variables.Clear();
			_names.Clear();
		}

		private void AddVariable(string name, Variable value)
		{
			_map.Add(name, _variables.Count);
			_variables.Add(value);
			_names.Add(name);
		}

		private void RemoveVariable(int index)
		{
			_map.Remove(_names[index]);
			_variables.RemoveAt(index);
			_names.RemoveAt(index);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;
		}

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
	}
}