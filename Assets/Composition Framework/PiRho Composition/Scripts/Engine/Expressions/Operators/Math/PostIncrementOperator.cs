using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class PostIncrementOperator : AssignOperator
	{
		private const string _missingAddException = "the list '{0}' cannot have values added";
		private const string _readOnlyAddException = "the list '{0}' is read only and cannot have values added";
		private const string _mismatchedAddException = "the list '{0}' cannot have empty values added";

		public override OperatorPrecedence Precedence => OperatorPrecedence.Postfix;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Symbol = parser.GetText(token);
		}

		public override Variable Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);

			if (left.IsList)
			{
				var result = left.AsList.AddVariable(Variable.Empty);

				if (result != SetVariableResult.Success)
				{
					switch (result)
					{
						case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAddException, Left);
						case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAddException, Left);
						case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAddException, Left);
					}
				}

				return left;

			}
			else
			{
				var value = VariableHandler.Add(left, Variable.Int(1));

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
