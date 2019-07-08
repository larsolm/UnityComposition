using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition.Engine
{
	internal class CommandOperation : Operation
	{
		private const string _missingCommandException = "a command named '{0}' does not exist";

		private string _name;
		private List<Operation> _parameters;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			_name = parser.GetText(token);
			_parameters = new List<Operation>();

			while (!parser.HasToken(ExpressionTokenType.EndGroup))
			{
				var parameter = parser.ParseLeft(OperatorPrecedence.Default);
				_parameters.Add(parameter);

				if (parser.HasToken(ExpressionTokenType.Separator))
					parser.SkipToken(ExpressionTokenType.Separator, ExpressionLexer.SeparatorSymbol.ToString());
			}

			parser.SkipToken(ExpressionTokenType.EndGroup, ExpressionLexer.GroupCloseSymbol.ToString());
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var command = ExpressionParser.GetCommand(_name);

			if (command != null)
				return command.Evaluate(variables, _name, _parameters);
			else
				throw new ExpressionEvaluationException(_missingCommandException, _name);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(_name);
			builder.Append(ExpressionLexer.GroupOpenSymbol);

			for (var i = 0; i < _parameters.Count; i++)
			{
				if (i != 0)
				{
					builder.Append(ExpressionLexer.SeparatorSymbol);
					builder.Append(' ');
				}

				_parameters[i].ToString(builder);
			}

			builder.Append(ExpressionLexer.GroupCloseSymbol);
		}

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			foreach (var parameter in _parameters)
				parameter.GetInputs(inputs, source);
		}
	}
}
