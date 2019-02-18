namespace PiRhoSoft.CompositionEngine
{
	public class InvertOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			if (result.Type == VariableType.Boolean)
				return VariableValue.Create(!result.Boolean);
			else
				throw new ExpressionEvaluationException(MismatchedBooleanType1Exception, '-', result.Type);
		}
	}
}
