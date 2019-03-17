namespace PiRhoSoft.CompositionEngine
{
	public class OrAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = OrOperator.Or(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
