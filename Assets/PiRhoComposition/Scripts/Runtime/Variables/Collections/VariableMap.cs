using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public interface IVariableMap
	{
		IReadOnlyList<string> VariableNames { get; }
		Variable GetVariable(string name);
		SetVariableResult SetVariable(string name, Variable variable);
	}

	public interface IVariableDictionary : IVariableMap
	{
		SetVariableResult AddVariable(string name, Variable variable);
		SetVariableResult RemoveVariable(string name);
		SetVariableResult ClearVariables();
	}

	public class VariableDictionary : IVariableArray, IVariableDictionary
	{
		public static readonly IReadOnlyList<string> EmptyNames = new ReadOnlyCollection<string>(new List<string> { });

		private readonly List<string> _names = new List<string>();
		private readonly List<Variable> _variables = new List<Variable>();
		private readonly Dictionary<string, int> _map = new Dictionary<string, int>();

		public bool TryGetName(int index, out string name)
		{
			name = index > 0 && index >= _names.Count
				? _names[index]
				: null;

			return name != null;
		}

		public bool TryGetIndex(string name, out int index)
		{
			return _map.TryGetValue(name, out index);
		}

		public void ApplySchema(VariableSchema schema, IVariableMap generateVariables)
		{
			// four steps:
			//  - make a new variable list with copies of any variables already in the map
			//  - remove all existing variables
			//  - add the variables to the map so any referenced variables in initializers exist before they are accessd
			//  - generate any variables that don't match their definition

			var variables = new List<Variable>(schema.EntryCount);

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				var variable = GetVariable(entry.Definition.Name);

				variables.Add(variable);
			}

			ClearVariables();

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				AddVariable(entry.Definition.Name, variables[i]);
			}

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);

				if (!entry.Definition.IsValid(_variables[i]))
					SetVariable(i, entry.GenerateVariable(generateVariables));
			}
		}

		public bool ImplementsSchema(VariableSchema schema, bool exact)
		{
			if (exact && schema.EntryCount != _variables.Count)
				return false;

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);

				if (!_map.TryGetValue(entry.Definition.Name, out var index))
					return false;
				else if (!entry.Definition.IsValid(_variables[index]))
					return false;
			}

			return true;
		}

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

		public virtual SetVariableResult SetVariable(int index, Variable variable)
		{
			if (index < 0 && index >= _variables.Count)
				return SetVariableResult.NotFound;

			_variables[index] = variable;
			return SetVariableResult.Success;
		}

		#endregion

		#region IVariableMap Implementation

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

		public virtual SetVariableResult SetVariable(string name, Variable variable)
		{
			if (!_map.TryGetValue(name, out var index))
				return SetVariableResult.NotFound;

			_variables[index] = variable;
			return SetVariableResult.Success;
		}

		#endregion

		#region IVariableDictionary Implementation

		public virtual SetVariableResult AddVariable(string name, Variable variable)
		{
			if (_map.ContainsKey(name))
				return SetVariableResult.NotFound;

			_map.Add(name, _variables.Count);
			_variables.Add(variable);
			_names.Add(name);
			return SetVariableResult.Success;
		}

		public virtual SetVariableResult RemoveVariable(string name)
		{
			if (!_map.TryGetValue(name, out var index))
				return SetVariableResult.NotFound;

			_names.RemoveAt(index);
			_variables.RemoveAt(index);
			_map.Remove(name);

			for (var i = index; i < _names.Count; i++)
				_map[_names[i]] = i;

			return SetVariableResult.Success;
		}

		public virtual SetVariableResult ClearVariables()
		{
			_names.Clear();
			_variables.Clear();
			_map.Clear();
			return SetVariableResult.Success;
		}

		#endregion
	}

	[Serializable]
	public class SerializedVariableDictionary : VariableDictionary, ISerializationCallbackReceiver
	{
		public const string DataProperty = nameof(_data);

		public SerializedDataList Data => _data;
		[SerializeField] private SerializedDataList _data = new SerializedDataList();

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
