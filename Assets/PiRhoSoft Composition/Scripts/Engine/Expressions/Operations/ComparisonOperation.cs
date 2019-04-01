using System;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class ComparisonOperation : InfixOperation
	{
		// Valid comparisons follow the same casting rules as laid out in the Casting region of the VariableValue
		// definition with the addition that VariableType Empty compares equal to null objects. Comparison results
		// follow the same rules as the .net CompareTo method.

		protected bool Equals(VariableValue left, VariableValue right)
		{
			var result = left.Handler.IsEqual(left, right);

			if (result.HasValue)
				return result.Value;
			else
				throw ComparisonMismatch(left.Type, right.Type);
		}

		protected int Compare(VariableValue left, VariableValue right)
		{
			var comparison = left.Handler.Compare(left, right);

			if (comparison.HasValue)
				return comparison.Value;
			else
				throw ComparisonMismatch(left.Type, right.Type);
		}

		private ExpressionEvaluationException ComparisonMismatch(VariableType left, VariableType right) => ExpressionEvaluationException.ComparisonTypeMismatch(Symbol, left, right);
	}
}
