namespace PiRhoSoft.CompositionEngine
{
	public class SubtractAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = SubtractOperator.Subtract(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
