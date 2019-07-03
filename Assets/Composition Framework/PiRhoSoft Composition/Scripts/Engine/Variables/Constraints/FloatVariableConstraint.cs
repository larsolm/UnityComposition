using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableConstraint : VariableConstraint
	{
		public bool HasRange = false;
		public float Minimum = 0;
		public float Maximum = 100;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(HasRange);
			writer.Write(Minimum);
			writer.Write(Maximum);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			try
			{
				HasRange = reader.ReadBoolean();
				Minimum = reader.ReadSingle();
				Maximum = reader.ReadSingle();
			}
			catch
			{
				HasRange = false;
				Minimum = 0;
				Maximum = 100;
			}
		}

		public override bool IsValid(VariableValue value)
		{
			return !HasRange || (value.Float >= Minimum && value.Float <= Maximum);
		}

		public override bool Equals(object obj)
		{
			return obj is FloatVariableConstraint other
				&& HasRange == other.HasRange
				&& Minimum == other.Minimum
				&& Maximum == other.Maximum;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
