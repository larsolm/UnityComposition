namespace PiRhoSoft.CompositionEngine
{
	public class InequalOperator : ComparisonOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return VariableValue.Create(!Equals(left, right));
		}
	}
}
