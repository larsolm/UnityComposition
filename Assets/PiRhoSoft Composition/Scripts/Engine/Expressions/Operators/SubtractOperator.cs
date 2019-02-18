namespace PiRhoSoft.CompositionEngine
{
	public class SubtractOperator : InfixOperation
	{
		public static VariableValue Subtract(ref VariableValue left, ref VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Integer:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Integer - right.Integer);
						case VariableType.Number: return VariableValue.Create(left.Integer - right.Number);
					}

					break;
				}
				case VariableType.Number:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Number - right.Integer);
						case VariableType.Number: return VariableValue.Create(left.Number - right.Number);
					}

					break;
				}
			}

			throw new ExpressionEvaluationException(MismatchedMathType2Exception, '-', left.Type, right.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Subtract(ref left, ref right);
		}
	}
}
