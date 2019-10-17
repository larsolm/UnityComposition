using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class GroupOperation : Operation
	{
		private Operation _group;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			_group = parser.ParseLeft(OperatorPrecedence.Default);
			parser.SkipToken(ExpressionTokenType.EndGroup, ExpressionLexer.GroupCloseSymbol.ToString());
		}

		public override Variable Evaluate(IVariableMap variables)
		{
			return _group.Evaluate(variables);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(ExpressionLexer.GroupOpenSymbol);
			_group.ToString(builder);
			builder.Append(ExpressionLexer.GroupCloseSymbol);
		}

		public override void GetInputs(VariableDefinitionList inputs, string source)
		{
			_group.GetInputs(inputs, source);
		}
	}
}
