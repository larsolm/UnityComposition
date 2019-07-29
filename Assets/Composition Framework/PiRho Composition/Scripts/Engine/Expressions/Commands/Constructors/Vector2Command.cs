using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector2Command : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);

				if (!xValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);

				if (!yValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				return Variable.Vector2(new Vector2(xValue.AsFloat, yValue.AsFloat));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
