using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableConstraint : VariableConstraint
	{
		public float Minimum;
		public float Maximum;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Minimum);
			writer.Write(Maximum);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			Minimum = reader.ReadSingle();
			Maximum = reader.ReadSingle();
		}

		public override bool IsValid(VariableValue value)
		{
			return value.Float >= Minimum && value.Float <= Maximum;
		}
	}
}
