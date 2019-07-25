using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class FloatConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Float;

		public float? Minimum = null;
		public float? Maximum = null;

		public FloatConstraint()
		{
		}

		public FloatConstraint(float? minimum, float? maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public override Variable Generate()
		{
			var minimum = Minimum ?? 0.0f;
			var maximum = Maximum ?? 0.0f;
			var value = Mathf.Clamp(0.0f, minimum, maximum);

			return Variable.Float(value);
		}

		public override bool IsValid(Variable variable)
		{
			if (variable.TryGetFloat(out var value))
			{
				if (Minimum.HasValue && value < Minimum.Value)
					return false;

				if (Maximum.HasValue && value > Maximum.Value)
					return false;

				return true;
			}

			return false;
		}

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write(Minimum.HasValue);
			writer.Write(Maximum.HasValue);

			if (Minimum.HasValue) writer.Write(Minimum.Value);
			if (Maximum.HasValue) writer.Write(Maximum.Value);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			var hasMinimum = reader.ReadBoolean();
			var hasMaximum = reader.ReadBoolean();

			Minimum = hasMinimum ? reader.ReadSingle() : (float?)null;
			Maximum = hasMaximum ? reader.ReadSingle() : (float?)null;
		}
	}
}