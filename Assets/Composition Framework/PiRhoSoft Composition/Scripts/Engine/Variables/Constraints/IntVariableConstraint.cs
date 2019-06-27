using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableConstraint : VariableConstraint
	{
		public bool HasRange = false;
		public int Minimum = 0;
		public int Maximum = 100;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(HasRange);
			writer.Write(Minimum);
			writer.Write(Maximum);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			HasRange = reader.ReadBoolean();
			Minimum = reader.ReadInt32();
			Maximum = reader.ReadInt32();
		}

		public override bool IsValid(VariableValue value)
		{
			return !HasRange || (value.Int >= Minimum && value.Int <= Maximum);
		}

		public override bool Equals(object obj)
		{
			return obj is IntVariableConstraint other
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
