namespace PiRhoSoft.CompositionEngine
{
	public class ExponentAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = ExponentOperator.Raise(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
