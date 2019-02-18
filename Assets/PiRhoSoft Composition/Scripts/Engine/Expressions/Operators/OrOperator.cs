namespace PiRhoSoft.CompositionEngine
{
	public class OrOperator : InfixOperation
	{
		public static VariableValue Or(ref VariableValue left, ref VariableValue right)
		{
			if (left.Type == VariableType.Boolean && right.Type == VariableType.Boolean)
				return VariableValue.Create(left.Boolean || right.Boolean);
			else
				throw new ExpressionEvaluationException(MismatchedBooleanType2Exception, "||", left.Type, right.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Or(ref left, ref right);
		}
	}
}
