namespace PiRhoSoft.Composition
{
	internal class SubtractAssignOperator : AssignmentOperator
	{
		private const string _readOnlyRemoveException = "the list '{0}' is read only and cannot have values removed";
		private const string _mismatchedRemoveException = "the list '{0}' cannot have values of type {1} removed";

		public override Variable Evaluate(IVariableMap variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			if (left.IsList)
			{
				var list = left.AsList;
				var removeIndex = -1;

				for (var i = 0; i < list.VariableCount; i++)
				{
					var item = list.GetVariable(i);
					var comparison = VariableHandler.IsEqual(item, right);

					if (comparison.HasValue && comparison.Value)
					{
						removeIndex = i;
						break;
					}
				}

				if (removeIndex >= 0)
				{
					var result = left.AsList.RemoveVariable(removeIndex);

					switch (result)
					{
						case SetVariableResult.Success: return Variable.Bool(true);
						case SetVariableResult.NotFound: return Variable.Bool(false);
						case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyRemoveException, Left);
						case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedRemoveException, Left, right.Type);
					}

					return Variable.Bool(true);
				}
				else
				{
					return Variable.Bool(false);
				}
			}
			else
			{
				var value = VariableHandler.Subtract(left, right);

				if (value.IsEmpty)
					throw TypeMismatch(left.Type, right.Type);

				return Assign(variables, value);
			}
		}
	}
}
