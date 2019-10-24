using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class CustomVariableCollection : SerializedVariableDictionary
	{
		public const string DefinitionsProperty = nameof(_definitions);

		[SerializeField] private VariableDefinitionList _definitions = new VariableDefinitionList();
		private bool _locked = false;

		protected override void Load()
		{
			_locked = false;
			base.Load();
			_locked = true;
		}

		public void Add(VariableDefinition definition)
		{
			_locked = false;

			if (AddVariable(definition.Name, definition.Generate()) == SetVariableResult.Success)
				_definitions.Add(definition);

			_locked = true;
		}

		public VariableDefinition GetDefinition(int index)
		{
			return index < 0 || index >= _definitions.Count ? null : _definitions[index];
		}

		#region IVariableArray Implementation

		public override SetVariableResult SetVariable(int index, Variable variable)
		{
			if (index < 0 || index >= _definitions.Count)
				return SetVariableResult.NotFound;
			
			if (!_definitions[index].IsValid(variable))
				return SetVariableResult.TypeMismatch;

			return base.SetVariable(index, variable);
		}

		#endregion

		#region IVariableMap Implementation

		public override SetVariableResult SetVariable(string name, Variable variable)
		{
			if (!TryGetIndex(name, out var index))
				return SetVariableResult.NotFound;

			if (!_definitions[index].IsValid(variable))
				return SetVariableResult.TypeMismatch;

			return base.SetVariable(name, variable);
		}

		#endregion

		#region IVariableDictionary Implementation

		public override SetVariableResult AddVariable(string name, Variable variable)
		{
			if (_locked)
				return SetVariableResult.ReadOnly;

			return base.AddVariable(name, variable);
		}

		public override SetVariableResult RemoveVariable(string name)
		{
			if (TryGetIndex(name, out var index))
				_definitions.RemoveAt(index);

			return base.RemoveVariable(name);
		}

		public override SetVariableResult ClearVariables()
		{
			_definitions.Clear();
			return base.ClearVariables();
		}

		#endregion
	}
}
