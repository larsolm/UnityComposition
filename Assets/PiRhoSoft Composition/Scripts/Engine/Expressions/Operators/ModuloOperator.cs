namespace PiRhoSoft.CompositionEngine
{
	public class ModuloOperator : InfixOperation
	{
		public static VariableValue Modulo(string symbol, ref VariableValue left, ref VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Int:
				{
					if (right.Int == 0)
						throw ExpressionEvaluationException.DivideByZero(symbol);

					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int % right.Int);
						case VariableType.Float: return VariableValue.Create(left.Float % right.Float);
					}

					break;
				}
				case VariableType.Float:
				{
					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int % right.Float);
						case VariableType.Float: return VariableValue.Create(left.Float % right.Float);
					}

					break;
				}
			}

			throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Int, VariableType.Float);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Modulo(Symbol, ref left, ref right);
		}
	}
}
