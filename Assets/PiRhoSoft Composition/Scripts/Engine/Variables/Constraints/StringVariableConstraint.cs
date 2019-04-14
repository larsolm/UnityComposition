using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StringVariableConstraint : VariableConstraint
	{
		public string[] Values;

		public override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Values.Length);

			foreach (var value in Values)
				writer.Write(value);
		}

		public override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var length = reader.ReadInt32();

			Values = new string[length];

			for (var i = 0; i < Values.Length; i++)
				Values[i] = reader.ReadString();
		}

		public override bool IsValid(VariableValue value)
		{
			return Values.Length == 0 || Values.Contains(value.String);
		}
	}
}
