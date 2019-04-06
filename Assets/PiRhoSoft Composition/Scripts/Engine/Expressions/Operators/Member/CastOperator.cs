namespace PiRhoSoft.CompositionEngine
{
	public class CastOperator : InfixOperation
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
			var value = _rightIdentifier.Cast(left);

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
	}
}
