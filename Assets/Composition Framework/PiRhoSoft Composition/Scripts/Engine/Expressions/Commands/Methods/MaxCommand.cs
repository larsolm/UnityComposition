using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	internal class MaxCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count >= 2)
			{
				var biggestValue = VariableValue.Empty;
				var biggestNumber = float.MinValue;

				for (var i = 0; i < parameters.Count; i++)
				{
					var p = parameters[i].Evaluate(variables);

					if (!p.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, i, p.Type, VariableType.Int, VariableType.Float);

					if (p.Number > biggestNumber)
					{
						biggestNumber = p.Number;
						biggestValue = p;
					}
				}

				return biggestValue;
			}
			else
			{
				throw CommandEvaluationException.TooFewParameters(name, parameters.Count, 2);
			}
		}
	}
}
