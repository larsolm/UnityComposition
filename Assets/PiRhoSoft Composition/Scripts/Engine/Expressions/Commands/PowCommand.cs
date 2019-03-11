using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class PowCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var value = parameters[0].Evaluate(variables);
				var power = parameters[1].Evaluate(variables);

				if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, value.Type, 0, VariableType.Number);

				if (power.Type != VariableType.Integer && power.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, power.Type, 1, VariableType.Number);

				return VariableValue.Create(Mathf.Pow(value.Number, power.Number));
			}
			else
			{
				throw new CommandEvaluationException(name, Command.WrongParameterCountException, parameters.Count, "s", 1);
			}
		}
	}
}
