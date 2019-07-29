using System.Text;

namespace PiRhoSoft.Composition
{
	internal interface ILookupOperation
	{
		Variable GetValue(IVariableCollection variables, Variable owner);
	}

	internal class LookupOperator : MemberOperator, ILookupOperation, IAssignableOperation
	{
		private const string _invalidLeftException = "the operator '{0}' expected a variable reference instead of '{1}'";
		private const string _invalidLookupException = "unable to find '{0}' on '{1}'";
		private const string _invalidAssignException = "unable to assign '{0}' to '{1}' on '{2}'";
		private const string _missingAssignmentException = "unable to assign '{0}' to '{1}' on '{2}'";
		private const string _readOnlyAssignmentException = "unable to assign '{0}' to '{1}' on read only '{2}'";
		private const string _mismatchedAssignmentException = "unable to assign '{0}' of type {1} to '{2}' on '{3}'";

		private ILookupOperation _leftLookup;

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			_leftLookup = Left as ILookupOperation;

			if (_leftLookup == null)
				throw new ExpressionParseException(token, _invalidLeftException, Symbol, Left);

			Right = parser.ParseLeft(OperatorPrecedence.Default);
			parser.SkipToken(ExpressionTokenType.EndLookup, ExpressionLexer.LookupCloseSymbol.ToString());
		}

		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Lookup(left, right);

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, right, left);

			return value;
		}

		public Variable GetValue(IVariableCollection variables, Variable owner)
		{
			var left = _leftLookup.GetValue(variables, owner);

			if (!left.IsEmpty)
			{
				var right = Right.Evaluate(variables);
				return VariableHandler.Lookup(left, right);
			}

			return left;
		}

		public SetVariableResult SetValue(IVariableCollection variables, Variable value)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var result = VariableHandler.Apply(ref left, right, value);

			if (result == SetVariableResult.Success && left.IsValueType)
			{
				if (Left is IAssignableOperation assignable)
					return assignable.SetValue(variables, value);
				else
					throw new ExpressionEvaluationException(_invalidAssignException, value, right, Left);
			}

			switch (result)
			{
				case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAssignmentException, value, right, left);
				case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAssignmentException, value, right, left);
				case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAssignmentException, value, value.Type, right, left);
			}

			return result;
		}

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(ExpressionLexer.LookupOpenSymbol);
			Right.ToString(builder);
			builder.Append(ExpressionLexer.LookupCloseSymbol);
		}
	}
}
