using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	internal class MaxCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count >= 2)
			{
				var biggestValue = Variable.Empty;
				var biggestNumber = float.MinValue;

				for (var i = 0; i < parameters.Count; i++)
				{
					var p = parameters[i].Evaluate(variables);

					if (!p.IsFloat)
						throw CommandEvaluationException.WrongParameterType(name, i, p.Type, VariableType.Int, VariableType.Float);

					if (p.AsFloat > biggestNumber)
					{
						biggestNumber = p.AsFloat;
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
