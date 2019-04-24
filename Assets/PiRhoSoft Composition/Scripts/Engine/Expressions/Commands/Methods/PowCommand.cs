using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class PowCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var value = parameters[0].Evaluate(variables);
				var power = parameters[1].Evaluate(variables);

				if (!value.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Int, VariableType.Float);

				if (!power.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, power.Type, VariableType.Int, VariableType.Float);

				return VariableValue.Create(Mathf.Pow(value.Number, power.Number));
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
			}
		}
	}
}
