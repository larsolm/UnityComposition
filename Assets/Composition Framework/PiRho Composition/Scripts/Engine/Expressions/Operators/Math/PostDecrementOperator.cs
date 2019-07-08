using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition.Engine
{
	internal class PostDecrementOperator : AssignOperator
	{
		private const string _missingRemoveException = "the list '{0}' is empty and cannot have values removed";
		private const string _readOnlyRemoveException = "the list '{0}' is read only and cannot have values removed";
		private const string _mismatchedRemoveException = "the list '{0}' cannot have values removed";

		public override OperatorPrecedence Precedence => OperatorPrecedence.Postfix;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Symbol = parser.GetText(token);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);

			if (left.HasList)
			{
				var result = left.List.Count > 0 ? left.List.RemoveVariable(left.List.Count - 1) : SetVariableResult.NotFound;

				if (result != SetVariableResult.Success)
				{
					switch (result)
					{
						case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingRemoveException, Left);
						case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyRemoveException, Left);
						case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedRemoveException, Left);
					}
				}

				return left;

			}
			else
			{
				var value = VariableHandler.Add(left, VariableValue.Create(-1));

				if (value.IsEmpty)
					throw TypeMismatch(left.Type, VariableType.Int);

				Assign(variables, value);
				return left;
			}
		}

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(Symbol);
		}

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			Left.GetInputs(inputs, source);
		}
	}
}
