using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ClampCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var value = parameters[0].Evaluate(variables);
				var min = parameters[1].Evaluate(variables);
				var max = parameters[2].Evaluate(variables);

				if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType2Exception, value.Type, 0, VariableType.Integer, VariableType.Number);

				if (min.Type != VariableType.Integer && min.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType2Exception, min.Type, 1, VariableType.Integer, VariableType.Number);

				if (max.Type != VariableType.Integer && max.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType2Exception, max.Type, 2, VariableType.Integer, VariableType.Number);

				if (min.Number > max.Number)
					throw new CommandEvaluationException(name, Command.InvalidRangeException, min.Number, max.Number);

				return VariableValue.Create(Mathf.Clamp(value.Number, min.Number, max.Number));
			}
			else
			{
				throw new CommandEvaluationException(name, Command.WrongParameterCountException, parameters.Count, parameters.Count == 1 ? "" : "s", 3);
			}
		}
	}
}
