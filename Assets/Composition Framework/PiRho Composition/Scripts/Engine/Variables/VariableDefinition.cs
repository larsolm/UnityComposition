using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable] public class VariableDefinitionList : SerializedList<VariableDefinition> { }

	[Serializable]
	public class VariableDefinition : ISerializationCallbackReceiver
	{
		public string Name;
		public VariableType Type { get => _type; set => SetType(value); }
		public VariableConstraint Constraint { get => _constraint; set => SetConstraint(value); }

		[SerializeField] private VariableType _type;
		private VariableConstraint _constraint;
		[SerializeField] private SerializedData _constraintData = new SerializedData();

		public VariableDefinition()
		{
			Name = string.Empty;
			Type = VariableType.Empty;
		}

		public VariableDefinition(string name)
		{
			Name = name;
			Type = VariableType.Empty;
		}

		public VariableDefinition(string name, VariableType type)
		{
			Name = name;
			Type = type;
		}

		public VariableDefinition(string name, VariableConstraint constraint)
		{
			Name = name;
			Constraint = constraint;
		}

		public Variable Generate()
		{
			return _constraint != null
				? _constraint.Generate()
				: Variable.Create(Type);
		}

		public bool IsValid(Variable variable)
		{
			return _constraint != null
				? _constraint.IsValid(variable)
				: variable.Is(Type);
		}

		private void SetType(VariableType type)
		{
			_type = type;
			_constraint = VariableConstraint.Create(type);
		}

		private void SetConstraint(VariableConstraint constraint)
		{
			_type = constraint?.Type ?? VariableType.Empty;
			_constraint = constraint;
		}

		public void OnBeforeSerialize() => _constraintData.Save(_constraint, 1);
		public void OnAfterDeserialize() => _constraintData.Load(out _constraint);
	}
}