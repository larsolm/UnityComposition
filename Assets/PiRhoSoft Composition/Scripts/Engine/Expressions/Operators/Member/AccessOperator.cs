using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class AccessOperator : InfixOperation, IAssignableOperation
	{
		private const string _invalidMemberAccessException = "the operator '{0}' expected an identifer instead of '{1}'";
		private const string _invalidMemberAssignException = "unable to assign '{0}' to '{1}'";

		private IdentifierOperation _rightIdentifier;

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			if (Left is IdentifierOperation leftIdentifier && leftIdentifier.Name == source)
				inputs.Add(new VariableDefinition { Name = _rightIdentifier.Name, Definition = ValueDefinition.Create(VariableType.Empty) });
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			base.Parse(parser, token);

			_rightIdentifier = Right as IdentifierOperation;

			if (_rightIdentifier == null)
				throw new ExpressionParseException(token, _invalidMemberAccessException, Symbol, Right);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			return _rightIdentifier.GetValue(left);
		}

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			var left = Left.Evaluate(variables);
			var result = _rightIdentifier.SetValue(ref left, value);

			if (result == SetVariableResult.Success && !left.HasReference)
			{
				if (Left is IAssignableOperation assignable)
					return assignable.SetValue(variables, value);
				else
					throw new ExpressionEvaluationException(_invalidMemberAssignException, value, Left);
			}

			return result;
		}
	}
}
