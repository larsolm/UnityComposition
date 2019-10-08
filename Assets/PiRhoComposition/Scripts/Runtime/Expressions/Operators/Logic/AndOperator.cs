namespace PiRhoSoft.Composition
{
	internal class AndOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.And;

		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);

			if (left.Type != VariableType.Bool)
				throw TypeMismatch(left.Type, VariableType.Bool);

			if (!left.AsBool)
				return Variable.Bool(false);

			var right = Right.Evaluate(variables);

			if (right.Type != VariableType.Bool)
				throw TypeMismatch(VariableType.Bool, right.Type);

			return Variable.Bool(right.AsBool);
		}
	}
}
