namespace PiRhoSoft.Composition
{
	internal class DivideAssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableCollection variables)
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
