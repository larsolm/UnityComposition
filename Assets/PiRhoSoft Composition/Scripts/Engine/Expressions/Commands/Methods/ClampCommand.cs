using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class ClampCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var value = parameters[0].Evaluate(variables);
				var min = parameters[1].Evaluate(variables);
				var max = parameters[2].Evaluate(variables);

				if (!value.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Int, VariableType.Float);

				if (!min.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, min.Type, VariableType.Int, VariableType.Float);

				if (!max.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, max.Type, VariableType.Int, VariableType.Float);

				if (value.Type == VariableType.Int && min.Type == VariableType.Int && max.Type == VariableType.Int)
					return VariableValue.Create(Mathf.Clamp(value.Int, min.Int, max.Int));
				else
					return VariableValue.Create(Mathf.Clamp(value.Number, min.Number, max.Number));
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
			}
		}
	}
}
