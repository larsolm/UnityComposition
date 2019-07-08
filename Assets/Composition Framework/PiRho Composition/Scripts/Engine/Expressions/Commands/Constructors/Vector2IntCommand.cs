using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class Vector2IntCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);

				if (xValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (yValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				return VariableValue.Create(new Vector2Int(xValue.Int, yValue.Int));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
