namespace PiRhoSoft.CompositionEngine
{
	public class ModuloAssignOperator : AssignOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);
			var result = ModuloOperator.Modulo(ref left, ref right);

			return Assign(variables, ref result);
		}
	}
}
