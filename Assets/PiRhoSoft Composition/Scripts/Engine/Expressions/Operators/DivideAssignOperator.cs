namespace PiRhoSoft.CompositionEngine
{
	public class DivideAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = DivideOperator.Divide(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
