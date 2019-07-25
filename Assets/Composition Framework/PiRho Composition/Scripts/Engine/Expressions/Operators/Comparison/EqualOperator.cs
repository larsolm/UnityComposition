namespace PiRhoSoft.Composition
{
	internal class EqualOperator : ComparisonOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Equality;

		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Variable.Bool(Equals(left, right));
		}
	}
}
