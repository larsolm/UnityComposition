using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class PrefixOperation : Operation
	{
		private const string _typeMismatchException = "the operator '{0}' cannot be applied to a value of type {1}";

		public string Symbol;
		public Operation Right;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Symbol = parser.GetText(token);
			Right = parser.ParseLeft(OperatorPrecedence.Prefix);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Symbol);
			Right.ToString(builder);
		}

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			Right.GetInputs(inputs, source);
		}

		protected ExpressionEvaluationException TypeMismatch(VariableType type)
		{
			return new ExpressionEvaluationException(_typeMismatchException, Symbol, type);
		}
	}
}
