namespace PiRhoSoft.Composition
{
	internal class InvertOperator : PrefixOperation
	{
		public override Variable Evaluate(IVariableMap variables)
		{
			var result = Right.Evaluate(variables);

			if (result.Type != VariableType.Bool)
				throw TypeMismatch(result.Type);

			return Variable.Bool(!result.AsBool);
		}
	}
}