namespace PiRhoSoft.CompositionEngine
{
	public class GreaterOperator : ComparisonOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return VariableValue.Create(Compare(left, right) > 0);
		}
	}
}
