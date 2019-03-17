namespace PiRhoSoft.CompositionEngine
{
	public class AddAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = AddOperator.Add(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
