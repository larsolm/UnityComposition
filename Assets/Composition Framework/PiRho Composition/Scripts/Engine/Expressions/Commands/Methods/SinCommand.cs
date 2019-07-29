using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class SinCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				if (result.TryGetFloat(out var number))
					return Variable.Float(Mathf.Sin(number));
				else
					throw CommandEvaluationException.WrongParameterType(name, 0, result.Type, VariableType.Float);
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1);
			}
		}
	}
}
