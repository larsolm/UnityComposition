using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class SignCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return Variable.Int(result.AsInt < 0 ? -1 : 1);
					case VariableType.Float: return Variable.Float(Mathf.Sign(result.AsFloat));
				}

				throw CommandEvaluationException.WrongParameterType(name, 0, result.Type, VariableType.Int, VariableType.Float);
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1);
			}
		}
	}
}
