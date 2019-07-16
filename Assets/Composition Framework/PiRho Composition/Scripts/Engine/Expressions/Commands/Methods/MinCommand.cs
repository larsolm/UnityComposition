using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	internal class MinCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count >= 2)
			{
				var smallestValue = VariableValue.Empty;
				var smallestNumber = float.MaxValue;

				for (var i = 0; i < parameters.Count; i++)
				{
					var p = parameters[i].Evaluate(variables);

					if (!p.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, i, p.Type, VariableType.Int, VariableType.Float);

					if (p.Number < smallestNumber)
					{
						smallestNumber = p.Number;
						smallestValue = p;
					}
				}

				return smallestValue;
			}
			else
			{
				throw CommandEvaluationException.TooFewParameters(name, parameters.Count, 2);
			}
		}
	}
}
