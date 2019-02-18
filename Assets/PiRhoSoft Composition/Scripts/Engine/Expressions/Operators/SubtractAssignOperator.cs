namespace PiRhoSoft.CompositionEngine
{
	public class SubtractAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = SubtractOperator.Subtract(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
