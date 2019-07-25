namespace PiRhoSoft.Composition
{
	internal class OrAssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);

			if (left.Type != VariableType.Bool)
				throw TypeMismatch(left.Type, VariableType.Bool);

			if (left.AsBool)
				return Variable.Bool(true);

			var right = Right.Evaluate(variables);

			if (right.Type != VariableType.Bool)
				throw TypeMismatch(VariableType.Bool, right.Type);

			return Assign(variables, Variable.Bool(right.AsBool));
		}
	}
}
