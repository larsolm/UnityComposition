using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	public class StringVariableConstraint : VariableConstraint
	{
		public List<string> Values = new List<string>();

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Values.Count);

			foreach (var value in Values)
				writer.Write(value ?? string.Empty);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var length = reader.ReadInt32();

			Values.Clear();

			for (var i = 0; i < length; i++)
				Values.Add(reader.ReadString());
		}

		public override bool IsValid(VariableValue value)
		{
			return Values.Count == 0 || Values.Contains(value.String);
		}

		public override bool Equals(object obj)
		{
			return obj is StringVariableConstraint other
				&& Values.SequenceEqual(other.Values);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
