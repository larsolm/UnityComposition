namespace PiRhoSoft.Composition
{
	internal class MultiplyOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Multiplication;

		public override Variable Evaluate(IVariableMap variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Multiply(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
