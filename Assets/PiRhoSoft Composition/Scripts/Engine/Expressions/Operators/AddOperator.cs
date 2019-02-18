namespace PiRhoSoft.CompositionEngine
{
	public class AddOperator : InfixOperation
	{
		public static VariableValue Add(Operation expression, ref VariableValue left, ref VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Integer:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Integer + right.Integer);
						case VariableType.Number: return VariableValue.Create(left.Integer + right.Number);
					}

					break;
				}
				case VariableType.Number:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Number + right.Integer);
						case VariableType.Number: return VariableValue.Create(left.Number + right.Number);
					}

					break;
				}
				case VariableType.String:
				{
					if (right.Type == VariableType.String)
						return VariableValue.Create(left.String + right.String);

					break;
				}
			}

			throw new ExpressionEvaluationException(MismatchedMathType2Exception, '+', left.Type, right.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Add(this, ref left, ref right);
		}
	}
}
