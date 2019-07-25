using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class CeilingCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				if (result.Type == VariableType.Float)
					return Variable.Int(Mathf.CeilToInt(result.AsFloat));
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
