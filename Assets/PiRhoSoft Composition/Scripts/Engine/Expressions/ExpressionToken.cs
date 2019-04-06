namespace PiRhoSoft.CompositionEngine
{
	public enum ExpressionTokenType
	{
		Sentinel,
		Constant,
		Int,
		Float,
		String,
		Color,
		Identifier,
		Command,
		Operator,
		StartLookup,
		EndLookup,
		StartGroup,
		EndGroup,
		Separator,
		Alternation,
		Unknown
	}

	public class ExpressionToken
	{
		public ExpressionTokenType Type;
		public int Location;
		public int Start;
		public int End;
	}
}
