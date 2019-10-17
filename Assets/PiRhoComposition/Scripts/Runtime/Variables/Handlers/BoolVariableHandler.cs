using PiRhoSoft.Utilities;

namespace PiRhoSoft.Composition
{
	internal class BoolVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsBool.ToString();
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			writer.Writer.Write(variable.AsBool);
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var b = reader.Reader.ReadBoolean();
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
