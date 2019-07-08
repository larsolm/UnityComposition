namespace PiRhoSoft.Composition.Engine
{
	internal class AndOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.And;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);

			if (left.Type != VariableType.Bool)
				throw TypeMismatch(left.Type, VariableType.Bool);

			if (!left.Bool)
				return VariableValue.Create(false);

			var right = Right.Evaluate(variables);

			if (right.Type != VariableType.Bool)
				throw TypeMismatch(VariableType.Bool, right.Type);

			return VariableValue.Create(right.Bool);
		}
	}
}
