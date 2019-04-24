namespace PiRhoSoft.CompositionEngine
{
	internal class DivideAssignOperator : AssignmentOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Divide(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return Assign(variables, value);
		}
	}
}
