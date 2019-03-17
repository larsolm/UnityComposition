namespace PiRhoSoft.CompositionEngine
{
	public class DivideAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = DivideOperator.Divide(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
