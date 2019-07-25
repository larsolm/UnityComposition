using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class EnumVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsEnum.ToString());
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			data.SaveEnum(writer, variable.AsEnum);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var e = data.LoadEnum(reader);
			return Variable.Enum(e);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetEnum(left.EnumType, out var e))
				return left.AsEnum == e;
			else
				return null;
		}

		protected internal override int? Compare_(Variable left, Variable right)
		{
			if (right.TryGetEnum(left.EnumType, out var e))
				return left.AsEnum.CompareTo(e);
			else
				return null;
		}
	}
}
