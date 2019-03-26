namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableConstraint : VariableConstraint
	{
		public float Minimum;
		public float Maximum;

		public override string Write()
		{
			return string.Format("{0}|{1}", Minimum, Maximum);
		}

		public override bool Read(string data)
		{
			var range = data.Split('|');

			return range.Length == 2
				&& float.TryParse(range[0], out Minimum)
				&& float.TryParse(range[1], out Maximum);
		}

		public override bool IsValid(VariableValue value)
		{
			return value.Float >= Minimum && value.Float <= Maximum;
		}
	}
}
