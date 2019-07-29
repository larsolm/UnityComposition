namespace PiRhoSoft.Composition
{
	internal class MultiplyAssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Multiply(left, right);

			if (value.IsEmpty)
				throw TypeMismatch(left.Type, right.Type);

			return Assign(variables, value);
		}
	}
}
