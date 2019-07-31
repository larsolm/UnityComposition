using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	internal class BoolVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsBool.ToString();
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

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetBool(out var t))
				return 1.0f;
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetBool(out var t))
				return Variable.Bool(time < 0.5f ? from.AsBool : t);
			else
				return Variable.Empty;
		}
	}
}