namespace PiRhoSoft.CompositionEngine
{
	public enum ExpressionTokenType
	{
		Sentinel,
		Boolean,
		Integer,
		Number,
		String,
		Null,
		Identifier,
		Command,
		Operator,
		StartGroup,
		EndGroup,
		Separator
	}

	public class ExpressionToken
	{
		public int Location;
		public ExpressionTokenType Type;
		public string Text;
		public int Integer;
		public float Number;
	}
}
