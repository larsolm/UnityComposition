namespace PiRhoSoft.CompositionEngine
{
	public class AndOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.And;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.And(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
