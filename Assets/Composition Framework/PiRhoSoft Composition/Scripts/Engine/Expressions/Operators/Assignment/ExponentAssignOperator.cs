namespace PiRhoSoft.CompositionEngine
{
	internal class ExponentAssignOperator : AssignmentOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Exponent(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return Assign(variables, value);
		}
	}
}
