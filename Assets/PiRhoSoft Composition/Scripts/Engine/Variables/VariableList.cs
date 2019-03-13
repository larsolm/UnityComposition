using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class VariableList : IVariableList, ISerializationCallbackReceiver
	{
		[SerializeField] private List<SerializedVariable> _data;
		[NonSerialized] private List<Variable> _variables = new List<Variable>();

		public void AddVariable(string name, VariableValue value)
		{
			_variables.Add(Variable.Create(name, value));
		}

		public void RemoveVariable(string name)
		{
			var index = GetVariableIndex(name);
			if (index >= 0)
				_variables.RemoveAt(index);
		}

		public int GetVariableIndex(string name)
		{
			for (var i = 0; i < _variables.Count; i++)
			{
				if (_variables[i].Name == name)
					return i;
			}

			return -1;
		}

		public void Clear()
		{
			_variables.Clear();
		}

		#region IVariableList Implementation

		public int VariableCount
		{
			get { return _variables.Count; }
		}

		public string GetVariableName(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Name : null;
		}

		public VariableValue GetVariableValue(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index].Value : VariableValue.Empty;
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			if (index >= 0 && index < _variables.Count)
			{
				_variables[index] = Variable.Create(_variables[index].Name, value);
				return SetVariableResult.Success;
			}
			else
			{
				return SetVariableResult.NotFound;
			}
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			_data = new List<SerializedVariable>(_variables.Count);

			var builder = new StringBuilder();

			foreach (var variable in _variables)
			{
				var data = new SerializedVariable();
				data.SetVariable(variable);
				_data.Add(data);
			}
		}

		public void OnAfterDeserialize()
		{
			_variables.Clear();

			if (_data != null)
			{
				foreach (var data in _data)
				{
					var variable = data.GetVariable();
					_variables.Add(variable);
				}
			}

			_data = null;
		}

		#endregion
	}
}
