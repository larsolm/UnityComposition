using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class AccessOperator : MemberOperator, ILookupOperation, IAssignableOperation
	{
		private const string _invalidLookupException = "unable to find variable '{0}' on '{1}'";
		private const string _invalidLeftException = "the operator '{0}' expected a variable reference instead of '{1}'";
		private const string _invalidRightException = "the operator '{0}' expected an identifer instead of '{1}'";
		private const string _invalidAssignException = "unable to assign '{0}' to '{1}'";

		private ILookupOperation _leftLookup;
		private IdentifierOperation _rightIdentifier;

		public override OperatorPrecedence Precedence => OperatorPrecedence.MemberAccess;

		public override void GetInputs(VariableDefinitionList inputs, string source)
		{
			if (Left is IdentifierOperation leftIdentifier && leftIdentifier.Name == source)
				inputs.Add(new VariableDefinition(_rightIdentifier.Name, VariableType.Empty));
			else
				base.GetInputs(inputs, source);
		}

		public override void GetOutputs(VariableDefinitionList outputs, string source)
		{
			if (Left is IdentifierOperation leftIdentifier && leftIdentifier.Name == source)
				outputs.Add(new VariableDefinition(_rightIdentifier.Name, VariableType.Empty));
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			_leftLookup = Left as ILookupOperation;

			if (_leftLookup == null)
				throw new ExpressionParseException(token, _invalidLeftException, Symbol, Left);

			base.Parse(parser, token);

			_rightIdentifier = Right as IdentifierOperation;

			if (_rightIdentifier == null)
				throw new ExpressionParseException(token, _invalidRightException, Symbol, Right);
		}

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(Symbol);
			Right.ToString(builder);
		}

		public override Variable Evaluate(IVariableCollection variables)
		{
			var left = Left.Evaluate(variables);
			var value = _rightIdentifier.GetValue(variables, left);

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, _rightIdentifier.Name, variables);

			return value;
		}

		public Variable GetValue(IVariableCollection variables, Variable owner)
		{
			var left = _leftLookup.GetValue(variables, owner);
			return left.IsEmpty ? left : _rightIdentifier.GetValue(variables, left);
		}

		public SetVariableResult SetValue(IVariableCollection variables, Variable value)
		{
			var left = Left.Evaluate(variables);
			var result = _rightIdentifier.SetValue(ref left, value);

			if (result == SetVariableResult.Success && left.IsValueType)
			{
				if (Left is IAssignableOperation assignable)
					return assignable.SetValue(variables, value);
				else
					throw new ExpressionEvaluationException(_invalidAssignException, value, Left);
			}

			return result;
		}
	}
}
