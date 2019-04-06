namespace PiRhoSoft.CompositionEngine
{
	public class OrOperator : InfixOperation
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Or;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Or(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return value;
		}
	}
}
