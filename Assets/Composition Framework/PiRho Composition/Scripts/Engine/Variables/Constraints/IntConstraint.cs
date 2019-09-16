﻿using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
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

			Minimum = hasMinimum ? reader.ReadInt32() : (int?)null;
			Maximum = hasMaximum ? reader.ReadInt32() : (int?)null;
		}
	}
}