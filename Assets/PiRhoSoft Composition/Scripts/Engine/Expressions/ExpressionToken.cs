using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum ExpressionTokenType
	{
		Sentinel,
		Boolean,
		Integer,
		Number,
		String,
		Color,
		Null,
		Identifier,
		Command,
		Operator,
		StartLookup,
		EndLookup,
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
		public Color Color;
	}
}
