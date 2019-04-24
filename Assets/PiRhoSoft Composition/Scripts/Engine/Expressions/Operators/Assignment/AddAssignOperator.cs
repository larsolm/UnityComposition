namespace PiRhoSoft.CompositionEngine
{
	internal class AddAssignOperator : AssignmentOperator
	{
		private const string _missingAddException = "the list '{0}' cannot have values added";
		private const string _readOnlyAddException = "the list '{0}' is read only and cannot have values added";
		private const string _mismatchedAddException = "the list '{0}' cannot have values of type {1} added";

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			if (left.HasList)
			{
				var result = left.List.AddVariable(right);

				if (result != SetVariableResult.Success)
				{
					switch (result)
					{
						case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAddException, Left);
						case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAddException, Left);
						case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAddException, Left, right.Type);
					}
				}

				return right;

			}
			else
			{
				var value = VariableHandler.Add(left, right);

				if (value.IsEmpty)
					throw TypeMismatch(left.Type, right.Type);

				return Assign(variables, value);
			}
		}
	}
}
