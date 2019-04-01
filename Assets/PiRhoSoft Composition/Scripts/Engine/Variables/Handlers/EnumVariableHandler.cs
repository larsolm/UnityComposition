using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableHandler : VariableHandler
	{
		protected override VariableConstraint CreateConstraint() => new EnumVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			if (constraint is EnumVariableConstraint enumConstraint)
				return VariableValue.Create((Enum)Enum.ToObject(enumConstraint.Type, 0));
			else
				return VariableValue.Empty;
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			// saving as a string is the simplest way of handling enums with non Int32 underlying type

			writer.Write(value.EnumType.AssemblyQualifiedName);
			writer.Write(value.Enum.ToString());
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var typeName = reader.ReadString();
			var enumValue = reader.ReadString();

			var type = Type.GetType(typeName);
			value = VariableValue.Create((Enum)Enum.Parse(type, enumValue));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return SetVariableResult.NotFound;
		}

		public static bool TryParse(Type enumType, string value, out Enum result)
		{
			try
			{
				result = (Enum)Enum.Parse(enumType, value);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		public override bool IsAssignable(VariableValue from, VariableValue to)
		{
			if (to.HasString)
				return TryParse(from.EnumType, to.String, out _);
			else
				return to.HasEnumType(from.EnumType);
		}

		public override bool? IsEqual(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum == right.Enum;
			else
				return null;
		}

		public override int? Compare(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum.CompareTo(right.Enum);
			else
				return null;
		}
	}
}
