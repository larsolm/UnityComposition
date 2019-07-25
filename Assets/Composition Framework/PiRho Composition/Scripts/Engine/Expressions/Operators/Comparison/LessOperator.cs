namespace PiRhoSoft.Composition
{
	internal class LessOperator : ComparisonOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Comparison;

		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Variable.Bool(Compare(left, right) < 0);
		}
	}
}
