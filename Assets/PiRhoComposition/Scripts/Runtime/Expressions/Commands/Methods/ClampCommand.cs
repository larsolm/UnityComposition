using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class ClampCommand : ICommand
	{
		public Variable Evaluate(IVariableMap variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var value = parameters[0].Evaluate(variables);
				var min = parameters[1].Evaluate(variables);
				var max = parameters[2].Evaluate(variables);

				if (!value.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Int, VariableType.Float);

				if (!min.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, min.Type, VariableType.Int, VariableType.Float);

				if (!max.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, max.Type, VariableType.Int, VariableType.Float);

				if (value.Type == VariableType.Int && min.Type == VariableType.Int && max.Type == VariableType.Int)
					return Variable.Int(Mathf.Clamp(value.AsInt, min.AsInt, max.AsInt));
				else
					return Variable.Float(Mathf.Clamp(value.AsFloat, min.AsFloat, max.AsFloat));
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
			}
		}
	}
}
