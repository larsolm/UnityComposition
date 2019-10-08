using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class PowCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var value = parameters[0].Evaluate(variables);
				var power = parameters[1].Evaluate(variables);

				if (!value.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Int, VariableType.Float);

				if (!power.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, power.Type, VariableType.Int, VariableType.Float);

				return Variable.Float(Mathf.Pow(value.AsFloat, power.AsFloat));
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
			}
		}
	}
}
