using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class BoolVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsBool);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			writer.Write(variable.AsBool);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var b = reader.ReadBoolean();
			return Variable.Bool(b);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetBool(out var b))
				return left.AsBool == b;
			else
				return null;
		}
	}
}