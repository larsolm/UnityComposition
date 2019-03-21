namespace PiRhoSoft.CompositionEngine
{
	public class SubtractAssignOperator : AssignmentOperation
	{
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
					if (ComparisonOperation.IsEqual(list.GetVariable(i), right))
					{
						removeIndex = i;
						break;
					}
				}

				if (removeIndex >= 0)
				{
					var result = left.List.RemoveVariable(removeIndex);
					var reference = Left is LookupOperation lookup ? lookup.Reference : null;

					switch (result)
					{
						case SetVariableResult.Success: return VariableValue.Create(true);
						case SetVariableResult.NotFound: return VariableValue.Create(false);
						case SetVariableResult.ReadOnly: throw ExpressionEvaluationException.ReadOnlyRemove(Symbol, reference);
						case SetVariableResult.TypeMismatch: throw ExpressionEvaluationException.MismatchedRemove(Symbol, reference, right.Type);
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
				var result = SubtractOperator.Subtract(Symbol, ref left, ref right);
				return Assign(variables, ref result);
			}
		}
	}
}
