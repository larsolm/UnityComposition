namespace PiRhoSoft.Composition
{
	internal class AndAssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);

			if (left.Type != VariableType.Bool)
				throw TypeMismatch(left.Type, VariableType.Bool);

			if (!left.AsBool)
				return Variable.Bool(false);

			var right = Right.Evaluate(variables);

			if (right.Type != VariableType.Bool)
				throw TypeMismatch(VariableType.Bool, right.Type);

			return Assign(variables, Variable.Bool(right.AsBool));
		}
	}
}
