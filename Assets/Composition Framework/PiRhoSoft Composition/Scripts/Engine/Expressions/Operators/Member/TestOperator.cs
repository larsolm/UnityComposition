namespace PiRhoSoft.CompositionEngine
{
	internal class TestOperator : MemberOperator
	{
		private const string _invalidLeftException = "the operator '{0}' expected a variable reference instead of '{1}'";
		private const string _invalidRightException = "the operator '{0}' expected a type name instead of '{1}'";

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			if (!(Left is ILookupOperation))
				throw new ExpressionParseException(token, _invalidLeftException, Symbol, Left);

			base.Parse(parser, token);

			if (!(Right is IdentifierOperation) && !(Right is TypeOperation))
				throw new ExpressionParseException(token, _invalidRightException, Symbol, Right);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = (Left as ILookupOperation).GetValue(variables, VariableValue.Create(variables));

			if (Right is IdentifierOperation identifier)
			{
				var result = VariableHandler.Test(left, identifier.Name);
				return VariableValue.Create(result);
			}
			else if (Right is TypeOperation type)
			{
				return VariableValue.Create(left.Type == type.Type);
			}

			return VariableValue.Empty;
		}
	}
}
