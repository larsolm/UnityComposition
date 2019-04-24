namespace PiRhoSoft.CompositionEngine
{
	internal class EqualOperator : ComparisonOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Equality;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			// if either side is the constant null, the other side is allowed to be empty - effectively an existance test
			// because the type of the caught exception isn't checked this is actually more generally a validity test

			if (Left is ConstantOperation leftConstant && leftConstant.Evaluate(variables).IsNull)
			{
				try
				{
					var right = Right.Evaluate(variables);
					return VariableValue.Create(right.IsNull || right.IsEmpty);
				}
				catch
				{
					return VariableValue.Create(true);
				}
			}
			else if (Right is ConstantOperation rightConstant && rightConstant.Evaluate(variables).IsNull)
			{
				try
				{
					var left = Left.Evaluate(variables);
					return VariableValue.Create(left.IsNull || left.IsEmpty);
				}
				catch
				{
					return VariableValue.Create(true);
				}
			}
			else
			{
				var left = Left.Evaluate(variables);
				var right = Right.Evaluate(variables);

				return VariableValue.Create(Equals(left, right));
			}
		}
	}
}
