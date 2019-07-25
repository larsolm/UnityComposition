using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector2IntCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);

				if (!xValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (!yValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				return Variable.Vector2Int(new Vector2Int(xValue.AsInt, yValue.AsInt));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
