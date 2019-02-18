using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class AsinCommand : Command
	{
		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Integer: return VariableValue.Create(Mathf.Asin(result.Integer));
					case VariableType.Number: return VariableValue.Create(Mathf.Asin(result.Number));
					default: throw new CommandEvaluationException(name, WrongParameterType1Exception, result.Type, 0, VariableType.Number);
				}
			}
			else
			{
				throw new CommandEvaluationException(name, WrongParameterCountException, parameters.Count, "s", 1);
			}
		}
	}
}
