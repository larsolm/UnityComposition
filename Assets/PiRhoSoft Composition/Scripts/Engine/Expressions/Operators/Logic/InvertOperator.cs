namespace PiRhoSoft.CompositionEngine
{
	internal class InvertOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			var value = VariableHandler.Not(result);

			if (value.IsEmpty)
				throw TypeMismatch(result.Type);

			return value;
		}
	}
}
