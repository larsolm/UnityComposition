using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class LogCommand : Command
	{
		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var value = parameters[0].Evaluate(variables);
				var power = 10.0f;

				if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, value.Type, 0, VariableType.Number);

				return VariableValue.Create(Mathf.Log(value.Number, power));
			}
			else if (parameters.Count == 2)
			{
				var value = parameters[0].Evaluate(variables);
				var power = parameters[1].Evaluate(variables);

				if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, value.Type, 0, VariableType.Number);

				if (power.Type != VariableType.Integer && power.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, power.Type, 1, VariableType.Number);

				return VariableValue.Create(Mathf.Log(value.Number, power.Number));
			}
			else
			{
				throw new CommandEvaluationException(name, WrongParameterRangeException, parameters.Count, "s", 1, 2);
			}
		}
	}
}
