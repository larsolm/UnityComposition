namespace PiRhoSoft.Composition
{
	internal class AddOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Addition;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Add(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
