using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LookupOperator : MemberOperator, IAssignableOperation
	{
		private const string _invalidLookupException = "unable to find '{0}' on '{1}'";
		private const string _invalidAssignException = "unable to assign '{0}' to '{1}' on '{2}'";
		private const string _missingAssignmentException = "unable to assign '{0}' to '{1}' on '{2}'";
		private const string _readOnlyAssignmentException = "unable to assign '{0}' to '{1}' on read only '{2}'";
		private const string _mismatchedAssignmentException = "unable to assign '{0}' of type {1} to '{2}' on '{3}'";

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Right = parser.ParseLeft(OperatorPrecedence.Default);
			parser.SkipToken(ExpressionTokenType.EndLookup, ExpressionLexer.LookupCloseSymbol.ToString());
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var value = VariableHandler.Lookup(left, right);

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, right, left);

			return value;
		}

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			var result = VariableHandler.Apply(ref left, right, value);

			if (result == SetVariableResult.Success && !left.HasReference)
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
