using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class SignCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return VariableValue.Create(result.Int < 0 ? -1 : 1);
					case VariableType.Float: return VariableValue.Create(Mathf.Sign(result.Float));
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
