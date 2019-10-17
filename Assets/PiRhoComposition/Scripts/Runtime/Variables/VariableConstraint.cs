using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class VariableConstraintAttribute : Attribute
	{
		public VariableType Type { get; private set; }
		public VariableConstraint Constraint { get; private set; }

		public VariableConstraintAttribute(VariableType type) => Type = type;
		public VariableConstraintAttribute(int minimum, int maximum) { Type = VariableType.Int; Constraint = new IntConstraint(minimum, maximum); }
		public VariableConstraintAttribute(bool noMinimum, int maximum) { Type = VariableType.Int; Constraint = new IntConstraint(null, maximum); }
		public VariableConstraintAttribute(int minimum, bool noMaximum) { Type = VariableType.Int; Constraint = new IntConstraint(minimum, null); }
		public VariableConstraintAttribute(float minimum, float maximum) { Type = VariableType.Float; Constraint = new FloatConstraint(minimum, maximum); }
		public VariableConstraintAttribute(bool noMinimum, float maximum) { Type = VariableType.Float; Constraint = new FloatConstraint(null, maximum); }
		public VariableConstraintAttribute(float minimum, bool noMaximum) { Type = VariableType.Float; Constraint = new FloatConstraint(minimum, null); }
		public VariableConstraintAttribute(string[] values) { Type = VariableType.String; Constraint = new StringConstraint(new List<string>(values)); }

		public VariableConstraintAttribute(Type type)
		{
			if (Variable.IsValidEnumType(type))
				Constraint = new EnumConstraint(type);
			else
				Constraint = new ObjectConstraint(type);
		}

		public VariableDefinition GetDefinition(string name)
		{
			if (Constraint != null)
				return new VariableDefinition(name, Constraint);
			else
				return new VariableDefinition(name, Type);
		}
	}

	[Serializable]
	public abstract class VariableConstraint
	{
		public abstract VariableType Type { get; }
		public abstract Variable Generate();
		public abstract bool IsValid(Variable value);
		public abstract void Save(SerializedDataWriter writer);
		public abstract void Load(SerializedDataReader reader);

		public static VariableConstraint Create(VariableType type)
		{
			switch (type)
			{
				case VariableType.Int: return new IntConstraint();
				case VariableType.Float: return new FloatConstraint();
				case VariableType.Enum: return new EnumConstraint();
				case VariableType.String: return new StringConstraint();
				case VariableType.List: return new ListConstraint();
				case VariableType.Dictionary: return new DictionaryConstraint();
				case VariableType.Asset: return new AssetConstraint();
				case VariableType.Object: return new ObjectConstraint();
				default: return null;
			}
		}
	}
}
