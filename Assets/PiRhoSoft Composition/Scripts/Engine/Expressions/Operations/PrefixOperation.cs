using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class PrefixOperation : Operation
	{
		public string Symbol;
		public Operation Right;

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Symbol);
			Right.ToString(builder);
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			Right.GetInputs(inputs, source);
		}

		protected ExpressionEvaluationException TypeMismatch(VariableType type, VariableType expected) => ExpressionEvaluationException.PrefixTypeMismatch(Symbol, type, expected);
		protected ExpressionEvaluationException TypeMismatch(VariableType type, VariableType expected1, VariableType expected2) => ExpressionEvaluationException.PrefixTypeMismatch(Symbol, type, expected1, expected2);
		protected ExpressionEvaluationException TypeMismatch(VariableType type, params VariableType[] expected) => ExpressionEvaluationException.PrefixTypeMismatch(Symbol, type, expected);
	}
}
