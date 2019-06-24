using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableConstraint : VariableConstraint
	{
		public bool HasRange;
		public int Minimum;
		public int Maximum;

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
	}
}
