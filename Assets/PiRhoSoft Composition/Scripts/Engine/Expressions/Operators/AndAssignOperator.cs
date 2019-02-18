namespace PiRhoSoft.CompositionEngine
{
	public class AndAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = AndOperator.And(this, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
