namespace PiRhoSoft.Composition
{
	internal class AssignOperator : AssignmentOperator
	{
		public override Variable Evaluate(IVariableMap variables)
		{
			var result = Right.Evaluate(variables);
			return Assign(variables, result);
		}
	}
}
