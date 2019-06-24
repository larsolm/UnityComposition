using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableConstraint : VariableConstraint
	{
		public bool HasRange;
		public float Minimum;
		public float Maximum;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(HasRange);
			writer.Write(Minimum);
			writer.Write(Maximum);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			HasRange = reader.ReadBoolean();
			Minimum = reader.ReadSingle();
			Maximum = reader.ReadSingle();
		}

		public override bool IsValid(VariableValue value)
		{
			return !HasRange || (value.Float >= Minimum && value.Float <= Maximum);
		}
	}
}
