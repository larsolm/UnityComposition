namespace PiRhoSoft.Composition
{
	internal class ExponentOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Exponentiation;

		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Exponent(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
