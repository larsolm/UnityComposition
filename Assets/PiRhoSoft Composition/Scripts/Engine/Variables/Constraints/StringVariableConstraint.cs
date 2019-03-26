namespace PiRhoSoft.CompositionEngine
{
	public class StringVariableConstraint : VariableConstraint
	{
		public string[] Values;

		public override string Write()
		{
			return string.Join(",", Values);
		}

		public override bool Read(string data)
		{
			Values = data.Split(',');
			return Values.Length > 0;
		}

		public override bool IsValid(VariableValue value)
		{
			throw new System.NotImplementedException();
		}
	}
}
