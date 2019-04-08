namespace PiRhoSoft.CompositionEngine
{
	public class TestOperator : MemberOperator
	{
		private const string _invalidTestException = "the operator '{0}' expected an identifer instead of '{1}'";

		private IdentifierOperation _rightIdentifier;

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			base.Parse(parser, token);

			_rightIdentifier = Right as IdentifierOperation;

			if (_rightIdentifier == null)
				throw new ExpressionParseException(token, _invalidTestException, Symbol, Right);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			return _rightIdentifier.Test(left);
		}
	}
}
