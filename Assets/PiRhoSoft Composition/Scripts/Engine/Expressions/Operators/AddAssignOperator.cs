namespace PiRhoSoft.CompositionEngine
{
	public class AddAssignOperator : AssignmentOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			if (left.HasList)
			{
				var result = left.List.AddVariable(right);

				if (result != SetVariableResult.Success)
				{
					var reference = Left is LookupOperation lookup ? lookup.Reference : null;

					switch (result)
					{
						case SetVariableResult.NotFound: throw ExpressionEvaluationException.MissingAdd(Symbol, reference);
						case SetVariableResult.ReadOnly: throw ExpressionEvaluationException.ReadOnlyAdd(Symbol, reference);
						case SetVariableResult.TypeMismatch: throw ExpressionEvaluationException.MismatchedAdd(Symbol, reference, right.Type);
					}
				}

				return right;

			}
			else
			{
				var result = AddOperator.Add(Symbol, ref left, ref right);
				return Assign(variables, ref result);
			}
		}
	}
}
