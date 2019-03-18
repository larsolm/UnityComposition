namespace PiRhoSoft.CompositionEngine
{
	public class ModuloAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = ModuloOperator.Modulo(Symbol, ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
