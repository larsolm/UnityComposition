namespace PiRhoSoft.Composition
{
	internal class InvertOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			if (result.Type != VariableType.Bool)
				throw TypeMismatch(result.Type);

			return VariableValue.Create(!result.Bool);
		}
	}
}