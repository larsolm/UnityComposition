using PiRhoSoft.Utilities;
using System.Collections.Generic;
using System.IO;

namespace PiRhoSoft.Composition
{
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

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write(Values.Count);

			foreach (var value in Values)
				writer.Write(value ?? string.Empty);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			Values.Clear();

			var length = reader.ReadInt32();

			for (var i = 0; i < length; i++)
				Values.Add(reader.ReadString());
		}

		private void SetValues(List<string> values)
		{
			_values = values ?? new List<string>();
		}
	}
}