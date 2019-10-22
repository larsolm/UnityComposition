using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class StringConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.String;

		public List<string> Values { get => _values; set => SetValues(value); }
		private List<string> _values;

		public StringConstraint()
		{
			Values = new List<string>();
		}

		public StringConstraint(List<string> values)
		{
			Values = values;
		}

		public override string ToString()
		{
			return string.Join(",", Values);
		}

		public override Variable Generate()
		{
			return Values.Count > 0
				? Variable.String(Values[0])
				: Variable.String(string.Empty);
		}

		public override bool IsValid(Variable value)
		{
			return value.IsString && (Values.Count == 0 || Values.Contains(value.AsString));
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.Writer.Write(Values.Count);

			foreach (var value in Values)
				writer.Writer.Write(value ?? string.Empty);
		}

		public override void Load(SerializedDataReader reader)
		{
			Values.Clear();

			var length = reader.Reader.ReadInt32();

			for (var i = 0; i < length; i++)
				Values.Add(reader.Reader.ReadString());
		}

		private void SetValues(List<string> values)
		{
			_values = values ?? new List<string>();
		}
	}
}
