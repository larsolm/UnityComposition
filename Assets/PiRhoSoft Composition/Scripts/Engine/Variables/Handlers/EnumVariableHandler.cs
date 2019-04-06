using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is EnumVariableConstraint enumConstraint)
				return VariableValue.Create((Enum)Enum.ToObject(enumConstraint.Type, 0));
			else
				return VariableValue.Empty;
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.ToString());
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			// saving as a string is the simplest way of handling enums with non Int32 underlying type

			writer.Write(value.EnumType.AssemblyQualifiedName);
			writer.Write(value.Enum.ToString());
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var typeName = reader.ReadString();
			var enumValue = reader.ReadString();

			var type = Type.GetType(typeName);

			try
			{
				var result = (Enum)Enum.Parse(type, enumValue);
				return VariableValue.Create(result);
			}
			catch
			{
				return VariableValue.Empty;
			}
		}

		protected override VariableConstraint CreateConstraint()
		{
			return new EnumVariableConstraint();
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum == right.Enum;
			else
				return null;
		}

		protected override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum.CompareTo(right.Enum);
			else
				return null;
		}
	}
}
