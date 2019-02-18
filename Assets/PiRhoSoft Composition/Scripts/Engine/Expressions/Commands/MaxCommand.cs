using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class MaxCommand : Command
	{
		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count < 2)
				throw new CommandEvaluationException(name, TooFewParametersException, parameters.Count, parameters.Count == 1 ? "" : "s", 2);

			var biggest = VariableValue.Create(int.MinValue);

			for (var i = 0; i < parameters.Count; i++)
			{
				var p = parameters[i].Evaluate(variables);

				if (p.Type != VariableType.Integer && p.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType2Exception, p.Type, i, VariableType.Integer, VariableType.Number);

				if (p > biggest)
					biggest = p;
			}

			return biggest;
		}
	}
}
