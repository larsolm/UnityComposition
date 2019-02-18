namespace PiRhoSoft.CompositionEngine
{
	public class NegateOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			switch (result.Type)
			{
				case VariableType.Integer: return VariableValue.Create(-result.Integer);
				case VariableType.Number: return VariableValue.Create(-result.Number);
				default: throw new ExpressionEvaluationException(MismatchedMathType1Exception, '-', result.Type);
			}
		}
	}
}
