using System.Text;

namespace PiRhoSoft.Composition
{
	internal class TernaryOperator : InfixOperation
	{
		private const string _invalidTernaryTypeException = "the operator '{0}' was passed a value of type {1} but can only operate on values of type {2}";

		private Operation _rightAlternative;

		public override OperatorPrecedence Precedence => OperatorPrecedence.Ternary;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			base.Parse(parser, token);
			parser.SkipToken(ExpressionTokenType.Alternation, ExpressionLexer.AlternationSymbol.ToString());
			_rightAlternative = parser.ParseRight(Precedence);
		}

		public override void ToString(StringBuilder builder)
		{
			base.ToString(builder);

			builder.Append(' ');
			builder.Append(ExpressionLexer.AlternationSymbol);
			builder.Append(' ');
			_rightAlternative.ToString(builder);
		}

		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);
			
			if (left.Type != VariableType.Bool)
				throw new ExpressionEvaluationException(_invalidTernaryTypeException, Symbol, left.Type, VariableType.Bool);

			if (left.AsBool)
				return Right.Evaluate(variables);
			else
				return _rightAlternative.Evaluate(variables);
		}
	}
}
