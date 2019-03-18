namespace PiRhoSoft.CompositionEngine
{
	public class MultiplyAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = MultiplyOperator.Multiply(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
