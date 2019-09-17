using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition
{
	public abstract class InfixOperation : Operation
	{
		private const string _typeMismatchException = "the operator '{0}' cannot be applied to values of type {1} and {2}";

		protected Operation Left;
		protected string Symbol;
		protected Operation Right;

		public abstract OperatorPrecedence Precedence { get; }

		internal void Setup(Operation left)
		{
			Left = left;
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Symbol = parser.GetText(token);
			Right = parser.ParseRight(Precedence);
		}

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(' ');
			builder.Append(Symbol);
			builder.Append(' ');
			Right.ToString(builder);
		}

		public override void GetInputs(VariableDefinitionList inputs, string source)
		{
			Left.GetInputs(inputs, source);
			Right.GetInputs(inputs, source);
		}

		protected ExpressionEvaluationException TypeMismatch(VariableType left, VariableType right)
		{
			return new ExpressionEvaluationException(_typeMismatchException, Symbol, left, right);
		}
	}
}
