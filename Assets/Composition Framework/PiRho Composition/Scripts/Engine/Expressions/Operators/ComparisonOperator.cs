namespace PiRhoSoft.Composition.Engine
{
	internal abstract class ComparisonOperator : InfixOperation
	{
		private const string _comparisonTypeMismatchException = "unable to compare types {0} and {1} with operator '{2}'";

		protected bool Equals(VariableValue left, VariableValue right)
		{
			var result = VariableHandler.IsEqual(left, right);

			if (result.HasValue)
				return result.Value;
			else
				throw ComparisonMismatch(left.Type, right.Type);
		}

		protected int Compare(VariableValue left, VariableValue right)
		{
			var comparison = VariableHandler.Compare(left, right);

			if (comparison.HasValue)
				return comparison.Value;
			else
				throw ComparisonMismatch(left.Type, right.Type);
		}

		private ExpressionEvaluationException ComparisonMismatch(VariableType left, VariableType right)
		{
			return new ExpressionEvaluationException(_comparisonTypeMismatchException, left, right, Symbol);
		}
	}
}
