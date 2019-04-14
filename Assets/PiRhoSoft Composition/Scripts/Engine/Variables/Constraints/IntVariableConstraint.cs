using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableConstraint : VariableConstraint
	{
		public int Minimum;
		public int Maximum;

		public override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Minimum);
			writer.Write(Maximum);
		}

		public override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			Minimum = reader.ReadInt32();
			Maximum = reader.ReadInt32();
		}

		public override bool IsValid(VariableValue value)
		{
			return value.Int >= Minimum && value.Int <= Maximum;
		}
	}
}
