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
			_constraint = GetConstraint(type);
		}

		private void SetConstraint(VariableConstraint constraint)
		{
			_type = GetType(constraint);
			_constraint = constraint;
		}

		private VariableConstraint GetConstraint(VariableType type)
		{
			switch (type)
			{
				case VariableType.Enum: return new EnumConstraint();
				case VariableType.Float: return new FloatConstraint();
				case VariableType.Int: return new IntConstraint();
				case VariableType.List: return new ListConstraint();
				case VariableType.Object: return new ObjectConstraint();
				case VariableType.Other: return new OtherConstraint();
				case VariableType.Store: return new StoreConstraint();
				case VariableType.String: return new StringConstraint();
				default: return null;
			}
		}

		private VariableType GetType(VariableConstraint constraint)
		{
			switch (constraint)
			{
				case EnumConstraint e: return VariableType.Enum;
				case FloatConstraint f: return VariableType.Float;
				case IntConstraint i: return VariableType.Int;
				case ListConstraint l: return VariableType.List;
				case ObjectConstraint o: return VariableType.Object;
				case OtherConstraint o: return VariableType.Other;
				case StoreConstraint s: return VariableType.Store;
				case StringConstraint s: return VariableType.String;
			}

			return VariableType.Empty;
		}

		public void OnBeforeSerialize() => _constraintData.Save(_constraint, 1);
		public void OnAfterDeserialize() => _constraintData.Load(out _constraint);
	}
}