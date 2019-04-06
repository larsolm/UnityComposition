namespace PiRhoSoft.CompositionEngine
{
	public class EqualOperator : ComparisonOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Equality;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			// if either side is the constant null, the other side is allowed to be empty - effectively an existance test
			// because the type of the caught exception isn't checked this is actually more generally a validity test

			VariableValue left = VariableValue.Empty;
			VariableValue right = VariableValue.Empty;

			if (Left is ConstantOperation leftConstant && leftConstant.Evaluate(variables).IsNull)
			{
				try
				{
					right = Right.Evaluate(variables);
				}
				catch
				{
				}
			}
			else if (Right is ConstantOperation rightConstant && rightConstant.Evaluate(variables).IsNull)
			{
				try
				{
					left = Left.Evaluate(variables);
				}
				catch
				{
				}
			}
			else
			{
				left = Left.Evaluate(variables);
				right = Right.Evaluate(variables);
			}

			return VariableValue.Create(Equals(left, right));
		}
	}
}
