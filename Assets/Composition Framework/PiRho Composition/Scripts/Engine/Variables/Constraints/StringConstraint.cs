using PiRhoSoft.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class StringConstraint : VariableConstraint
	{
		public List<string> Values;
		
		public StringConstraint()
		{
			Values = null;
		}

		public StringConstraint(List<string> values)
		{
			Values = values;
		}

		public override Variable Generate()
		{
			return Values != null && Values.Count > 0
				? Variable.String(Values[0])
				: Variable.String(string.Empty);
		}

		public override bool IsValid(Variable value)
		{
			return value.IsString && (Values == null || Values.Count == 0 || Values.Contains(value.AsString));
		}

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write(Values != null ? Values.Count : 0);

			if (Values != null)
			{
				foreach (var value in Values)
					writer.Write(value ?? string.Empty);
			}
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			var length = reader.ReadInt32();

			if (length == 0)
			{
				Values = null;
			}
			else
			{
				if (Values == null)
					Values = new List<string>(length);
				else
					Values.Clear();

				for (var i = 0; i < length; i++)
					Values.Add(reader.ReadString());
			}
		}
	}
}