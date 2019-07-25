namespace PiRhoSoft.Composition
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

		public override Variable Evaluate(IVariableStore variables)
		{
			var left = (Left as ILookupOperation).GetValue(variables, Variable.Store(variables));

			if (Right is IdentifierOperation identifier)
			{
				var result = VariableHandler.Test(left, identifier.Name);
				return Variable.Bool(result);
			}
			else if (Right is TypeOperation type)
			{
				return Variable.Bool(left.Type == type.Type);
			}

			return Variable.Empty;
		}
	}
}
