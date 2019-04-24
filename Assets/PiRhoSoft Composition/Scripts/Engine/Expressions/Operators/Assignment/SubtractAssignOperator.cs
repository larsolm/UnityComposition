namespace PiRhoSoft.CompositionEngine
{
	internal class SubtractAssignOperator : AssignmentOperator
	{
		private const string _readOnlyRemoveException = "the list '{0}' is read only and cannot have values removed";
		private const string _mismatchedRemoveException = "the list '{0}' cannot have values of type {1} removed";

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			if (left.HasList)
			{
				var list = left.List;
				var removeIndex = -1;

				for (var i = 0; i < list.Count; i++)
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
					var result = left.List.RemoveVariable(removeIndex);

					switch (result)
					{
						case SetVariableResult.Success: return VariableValue.Create(true);
						case SetVariableResult.NotFound: return VariableValue.Create(false);
						case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyRemoveException, Left);
						case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedRemoveException, Left, right.Type);
					}

					return VariableValue.Create(true);
				}
				else
				{
					return VariableValue.Create(false);
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
