namespace PiRhoSoft.CompositionEngine
{
	public class MultiplyAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = MultiplyOperator.Multiply(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
