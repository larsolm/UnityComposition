namespace PiRhoSoft.CompositionEngine
{
	public class OrOperator : InfixOperation
	{
		public static VariableValue Or(string symbol, ref VariableValue left, ref VariableValue right)
		{
			if (left.Type == VariableType.Bool && right.Type == VariableType.Bool)
				return VariableValue.Create(left.Bool || right.Bool);
			else
				throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Bool);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Or(Symbol, ref left, ref right);
		}
	}
}
