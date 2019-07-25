namespace PiRhoSoft.Composition
{
	internal class NegateOperator : PrefixOperation
	{
		public override Variable Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			var value = VariableHandler.Negate(result);

			if (value.IsEmpty)
				throw TypeMismatch(result.Type);

			return value;
		}
	}
}
