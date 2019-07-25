namespace PiRhoSoft.Composition
{
	internal class ModuloOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Multiplication;

		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Modulo(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
