namespace PiRhoSoft.CompositionEngine
{
	public class AddAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = AddOperator.Add(this, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
