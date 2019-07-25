namespace PiRhoSoft.Composition
{
	internal class AssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			return Assign(variables, result);
		}
	}
}
