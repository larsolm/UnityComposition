namespace PiRhoSoft.CompositionEngine
{
	public class ModuloOperator : InfixOperation
	{
		private const string _invalidDivisionException = "cannot divide by 0";

		public static VariableValue Modulo(ref VariableValue left, ref VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Integer:
				{
					if (right.Integer == 0)
						throw new ExpressionEvaluationException(_invalidDivisionException);

					switch (left.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Integer % right.Integer);
						case VariableType.Number: return VariableValue.Create(left.Number % right.Integer);
					}

					break;
				}
				case VariableType.Number:
				{
					switch (left.Type)
					{
						case VariableType.Integer: return VariableValue.Create(left.Integer % right.Number);
						case VariableType.Number: return VariableValue.Create(left.Number % right.Number);
					}

					break;
				}
			}

			throw new ExpressionEvaluationException(MismatchedMathType2Exception, '%', left.Type, right.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Modulo(ref left, ref right);
		}
	}
}
