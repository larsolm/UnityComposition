namespace PiRhoSoft.Composition.Engine
{
	internal class DecrementOperator : PrefixOperation
	{
		private const string _invalidAssignmentException = "unable to decrement '{0}'";

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			var value = VariableHandler.Add(result, VariableValue.Create(-1));

			if (value.IsEmpty)
				throw TypeMismatch(result.Type);

			if (Right is IAssignableOperation assignable)
			{
				assignable.SetValue(variables, value);
				return value;
			}
			else
			{
				throw new ExpressionEvaluationException(_invalidAssignmentException, Right);
			}
		}
	}
}
