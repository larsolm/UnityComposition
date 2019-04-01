namespace PiRhoSoft.CompositionEngine
{
	public class EqualOperator : ComparisonOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			// if either side is the literal null, the other side is allowed to be empty (effectively an existance test)

			VariableValue left = VariableValue.Empty;
			VariableValue right = VariableValue.Empty;

			if (Left is LiteralOperation leftLiteral && leftLiteral.Value.IsEmpty)
			{
				left = leftLiteral.Value;

				try
				{
					right = Right.Evaluate(variables);
				}
				catch
				{
					if (!leftLiteral.Value.IsNull)
						throw;
				}
			}
			else if (Right is LiteralOperation rightLiteral)
			{
				right = rightLiteral.Value;

				try
				{
					left = Left.Evaluate(variables);
				}
				catch
				{
					if (!rightLiteral.Value.IsNull)
						throw;
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
