using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionCommand : Command
	{
		public const string ParameterName = "P";

		public Expression Expression { get; private set; }

		public ExpressionCommand(Expression expression) => Expression = expression;

		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			for (var i = 0; i < parameters.Count; i++)
			{
				var parameterName = ParameterName + (i + 1);
				var parameterValue = parameters[i].Evaluate(variables);

				variables.SetVariable(parameterName, parameterValue);
			}

			return Expression.Evaluate(variables);
		}
	}
}
