using PiRhoSoft.Utilities;
using System;
using System.IO;

namespace PiRhoSoft.Composition
{
	// TODO: Could do something fancy here where any enum type that has a definition gets a cache created holding its
	// boxed values so they can be looked up (as opposed to doing a boxing conversion) at runtime.

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

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			data.SaveType(writer, EnumType);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			EnumType = data.LoadType(reader);
		}
	}
}