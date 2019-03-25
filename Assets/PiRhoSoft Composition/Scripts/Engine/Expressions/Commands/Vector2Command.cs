using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector2Command : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);

				if (!xValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);

				if (!yValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				return VariableValue.Create(new Vector2(xValue.Number, yValue.Number));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
