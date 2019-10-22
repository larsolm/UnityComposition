﻿using PiRhoSoft.Utilities;
using System;

namespace PiRhoSoft.Composition
{
	// TODO: Could do something fancy here where any enum type that has a definition gets a cache created holding its
	// boxed values so they can be looked up (as opposed to doing a boxing conversion) at runtime.

	[Serializable]
	public class EnumConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Enum;

		private Type _enumType;

		public Type EnumType
		{
			get => _enumType;
			set => _enumType = Variable.IsValidEnumType(value) ? value : typeof(Variable.InvalidEnum);
		}

		public EnumConstraint()
		{
			EnumType = null;
		}

		public EnumConstraint(Type type)
		{
			EnumType = type;
		}

		public override string ToString()
		{
			return _enumType.Name;
		}

		public override Variable Generate()
		{
			var values = EnumType.GetEnumValues();
			return Variable.Enum(values.GetValue(0) as Enum);
		}

		public override bool IsValid(Variable variable)
		{
			return variable.HasEnum(EnumType);
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.SaveType(EnumType);
		}

		public override void Load(SerializedDataReader reader)
		{
			EnumType = reader.LoadType();
		}
	}
}
