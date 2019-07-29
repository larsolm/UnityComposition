using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class QuaternionCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);

				if (!xValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);

				if (!yValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				if (!zValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Float);

				return Variable.Quaternion(Quaternion.Euler(xValue.AsFloat, yValue.AsFloat, zValue.AsFloat));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
		}
	}
}
