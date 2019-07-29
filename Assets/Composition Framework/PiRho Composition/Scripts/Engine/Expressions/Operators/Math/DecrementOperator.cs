namespace PiRhoSoft.Composition
{
	internal class DecrementOperator : PrefixOperation
	{
		private const string _invalidAssignmentException = "unable to decrement '{0}'";

		public override Variable Evaluate(IVariableCollection variables)
		{
			var result = Right.Evaluate(variables);
			var value = VariableHandler.Add(result, Variable.Int(-1));

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
