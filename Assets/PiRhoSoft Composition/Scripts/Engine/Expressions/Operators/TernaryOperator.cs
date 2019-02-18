using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class TernaryOperator : Operation
	{
		private const string _invalidTernaryTypeException = "the operator '?' was passed a value of type {0} but can only operate on values of type Boolean";

		public Operation Condition;
		public Operation TrueBranch;
		public Operation FalseBranch;

		public override void ToString(StringBuilder builder)
		{
			Condition.ToString(builder);
			builder.Append(" ? ");
			TrueBranch.ToString(builder);
			builder.Append(" : ");
			FalseBranch.ToString(builder);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Condition.Evaluate(variables);
			
			if (left.Type != VariableType.Boolean)
				throw new ExpressionEvaluationException(_invalidTernaryTypeException, left.Type);

			if (left.Boolean)
				return TrueBranch.Evaluate(variables);
			else
				return FalseBranch.Evaluate(variables);
		}
	}
}
