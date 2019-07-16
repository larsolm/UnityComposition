namespace PiRhoSoft.Composition
{
	internal class CastOperator : MemberOperator, ILookupOperation
	{
		private const string _invalidCastException = "the operator '{0}' expected an identifer instead of '{1}'";
		private const string _invalidAssignException = "unable to assign '{0}' as '{1}' to '{2}'";

		private IdentifierOperation _rightIdentifier;

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			base.Parse(parser, token);

			_rightIdentifier = Right as IdentifierOperation;

			if (_rightIdentifier == null)
				throw new ExpressionParseException(token, _invalidCastException, Symbol, Right);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var value = VariableHandler.Cast(left, _rightIdentifier.Name);

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidCastException, left, _rightIdentifier.Name);

			if (!value.IsEmpty && !left.HasReference)
			{
				if (Left is IAssignableOperation assignable)
				{
					var result = assignable.SetValue(variables, value);

					if (result == SetVariableResult.Success)
						return value;
				}

				throw new ExpressionEvaluationException(_invalidAssignException, value, left, Left);
			}

			return value;
		}

		public VariableValue GetValue(IVariableStore variables, VariableValue owner)
		{
			var left = Left is ILookupOperation lookup ? lookup.GetValue(variables, owner) : VariableValue.Empty;
			return left.IsEmpty ? left : VariableHandler.Cast(left, _rightIdentifier.Name);
		}
	}
}
