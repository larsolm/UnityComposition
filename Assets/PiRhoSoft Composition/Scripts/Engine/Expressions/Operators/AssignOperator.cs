namespace PiRhoSoft.CompositionEngine
{
	public class AssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			return Assign(variables, ref result);
		}
	}
}
