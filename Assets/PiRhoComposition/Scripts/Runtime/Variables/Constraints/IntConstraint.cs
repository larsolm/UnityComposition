using PiRhoSoft.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class IntConstraint : VariableConstraint
	{
		public const int DefaultMinimum = 0;
		public const int DefaultMaximum = 10;

		public override VariableType Type => VariableType.Int;

		public int? Minimum = null;
		public int? Maximum = null;

		public IntConstraint()
		{
		}

		public IntConstraint(int? minimum, int? maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public override string ToString()
		{
			return string.Format($"{Minimum?.ToString() ?? string.Empty},{Maximum?.ToString() ?? string.Empty}");
		}

		public override Variable Generate()
		{
			var minimum = Minimum ?? DefaultMinimum;
			var maximum = Maximum ?? DefaultMaximum;
			var value = Mathf.Clamp(0, minimum, maximum);

			return Variable.Int(value);
		}

		public override bool IsValid(Variable variable)
		{
			if (variable.TryGetInt(out var value))
			{
				if (Minimum.HasValue && value < Minimum.Value)
					return false;

				if (Maximum.HasValue && value > Maximum.Value)
					return false;

				return true;
			}

			return false;
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.Writer.Write(Minimum.HasValue);
			writer.Writer.Write(Maximum.HasValue);

			if (Minimum.HasValue) writer.Writer.Write(Minimum.Value);
			if (Maximum.HasValue) writer.Writer.Write(Maximum.Value);
		}

		public override void Load(SerializedDataReader reader)
		{
			var hasMinimum = reader.Reader.ReadBoolean();
			var hasMaximum = reader.Reader.ReadBoolean();

			Minimum = hasMinimum ? reader.Reader.ReadInt32() : (int?)null;
			Maximum = hasMaximum ? reader.Reader.ReadInt32() : (int?)null;
		}
	}
}
