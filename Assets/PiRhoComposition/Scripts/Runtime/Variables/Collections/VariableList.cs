using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public interface IVariableArray
	{
		int VariableCount { get; }
		Variable GetVariable(int index);
		SetVariableResult SetVariable(int index, Variable variable);
	}

	public interface IVariableList : IVariableArray
	{
		SetVariableResult AddVariable(Variable variable);
		SetVariableResult RemoveVariable(int index);
		SetVariableResult InsertVariable(int index, Variable variable);
		SetVariableResult ClearVariables();
	}

	public class VariableList : IVariableList
	{
		private readonly List<Variable> _variables = new List<Variable>();

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
			if (index < 0 || index >= _variables.Count)
				return SetVariableResult.NotFound;

			_variables[index] = variable;
			return SetVariableResult.Success;
		}

		#endregion

		#region IVariableList Implementation

		public virtual SetVariableResult AddVariable(Variable variable)
		{
			_variables.Add(variable);
			return SetVariableResult.Success;
		}

		public virtual SetVariableResult RemoveVariable(int index)
		{
			if (index < 0 || index >= _variables.Count)
				return SetVariableResult.NotFound;

			_variables.RemoveAt(index);
			return SetVariableResult.Success;
		}

		public virtual SetVariableResult InsertVariable(int index, Variable variable)
		{
			if (index < 0 || index > _variables.Count)
				return SetVariableResult.NotFound;

			_variables.Insert(index, variable);
			return SetVariableResult.Success;
		}

		public virtual SetVariableResult ClearVariables()
		{
			_variables.Clear();
			return SetVariableResult.Success;
		}

		#endregion
	}

	[Serializable]
	public class SerializedVariableList : VariableList, ISerializationCallbackReceiver
	{
		public const string DataProperty = nameof(_data);

		[SerializeField] private SerializedDataList _data = new SerializedDataList();

		private void Save()
		{
			_data.Clear();

			for (var i = 0; i < VariableCount; i++)
			{
				using (var writer = new SerializedDataWriter(_data))
					VariableHandler.Save(GetVariable(i), writer);
			}
		}

		private void Load()
		{
			ClearVariables();

			for (var i = 0; i < _data.Count; i++)
			{
				using (var reader = new SerializedDataReader(_data, i))
				{
					var variable = VariableHandler.Load(reader);
					AddVariable(variable);
				}
			}
		}

		#region ISerializationCallbackReceiver Implementation

		void ISerializationCallbackReceiver.OnBeforeSerialize() { }
		void ISerializationCallbackReceiver.OnAfterDeserialize() => Load();

		#endregion

		#region Editor Overrides

#if UNITY_EDITOR
		public override SetVariableResult AddVariable(Variable variable)
		{
			var result = base.AddVariable(variable);
			Save();
			return result;
		}

		public override SetVariableResult ClearVariables()
		{
			var result = base.ClearVariables();
			Save();
			return result;
		}

		public override SetVariableResult RemoveVariable(int index)
		{
			var result = base.RemoveVariable(index);
			Save();
			return result;
		}

		public override SetVariableResult InsertVariable(int index, Variable variable)
		{
			var result = base.InsertVariable(index, variable);
			Save();
			return result;
		}

		public override SetVariableResult SetVariable(int index, Variable variable)
		{
			var result = base.SetVariable(index, variable);
			Save();
			return result;
		}
#endif

		#endregion
	}
}
