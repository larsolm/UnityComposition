using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
{
	internal class EnumVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is EnumVariableConstraint enumConstraint)
				return VariableValue.Create((Enum)Enum.ToObject(enumConstraint.Type, 0));
			else
				return VariableValue.Empty;
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.ToString());
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			// saving as a string is the simplest way of handling enums with non Int32 underlying type
			// it also allows reordering/adding/removing of enum values without affecting saved data

			writer.Write(value.EnumType.AssemblyQualifiedName);
			writer.Write(value.Enum.ToString());
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
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

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum == right.Enum;
			else
				return null;
		}

		protected internal override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.HasEnumType(left.EnumType))
				return left.Enum.CompareTo(right.Enum);
			else
				return null;
		}
	}
}
