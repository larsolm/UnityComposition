using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
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

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			if (Left is IdentifierOperation leftIdentifier && leftIdentifier.Name == source)
				inputs.Add(new VariableDefinition { Name = _rightIdentifier.Name, Definition = ValueDefinition.Create(VariableType.Empty) });
			else
				base.GetInputs(inputs, source);
		}

		public override void GetOutputs(IList<VariableDefinition> outputs, string source)
		{
			if (Left is IdentifierOperation leftIdentifier && leftIdentifier.Name == source)
				outputs.Add(new VariableDefinition { Name = _rightIdentifier.Name, Definition = ValueDefinition.Create(VariableType.Empty) });
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

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var value = _rightIdentifier.GetValue(variables, left);

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, _rightIdentifier.Name, variables);

			return value;
		}

		public VariableValue GetValue(IVariableStore variables, VariableValue owner)
		{
			var left = _leftLookup.GetValue(variables, owner);
			return left.IsEmpty ? left : _rightIdentifier.GetValue(variables, left);
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
					throw new ExpressionEvaluationException(_invalidAssignException, value, Left);
			}

			return result;
		}
	}
}
