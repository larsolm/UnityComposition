using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class InfixOperation : Operation
	{
		public Operation Left;
		public string Symbol;
		public Operation Right;

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(' ');
			builder.Append(Symbol);
			builder.Append(' ');
			Right.ToString(builder);
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			Left.GetInputs(inputs, source);
			Right.GetInputs(inputs, source);
		}

		protected ExpressionEvaluationException TypeMismatch(VariableType left, VariableType right, VariableType expected) => ExpressionEvaluationException.InfixTypeMismatch(Symbol, left, right, expected);
		protected ExpressionEvaluationException TypeMismatch(VariableType left, VariableType right, VariableType expected1, VariableType expected2) => ExpressionEvaluationException.InfixTypeMismatch(Symbol, left, right, expected1, expected2);
		protected ExpressionEvaluationException TypeMismatch(VariableType left, VariableType right, params VariableType[] expected) => ExpressionEvaluationException.InfixTypeMismatch(Symbol, left, right, expected);
	}
}
