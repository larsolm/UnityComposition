namespace PiRhoSoft.Composition
{
	internal class LessOrEqualOperator : ComparisonOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Comparison;

		public override Variable Evaluate(IVariableMap variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Variable.Bool(Compare(left, right) <= 0);
		}
	}
}
