namespace PiRhoSoft.CompositionEngine
{
	public class AndOperator : InfixOperation
	{
		public static VariableValue And(string symbol, ref VariableValue left, ref VariableValue right)
		{
			if (left.Type == VariableType.Bool && right.Type == VariableType.Bool)
				return VariableValue.Create(left.Bool && right.Bool);
			else
				throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Bool);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return And(Symbol, ref left, ref right);
		}
	}
}
