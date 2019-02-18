namespace PiRhoSoft.CompositionEngine
{
	public class OrAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = OrOperator.Or(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
