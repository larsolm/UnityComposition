namespace PiRhoSoft.CompositionEngine
{
	public class InvertOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			if (result.Type == VariableType.Bool)
				return VariableValue.Create(!result.Bool);
			else
				throw TypeMismatch(result.Type, VariableType.Bool);
		}
	}
}
