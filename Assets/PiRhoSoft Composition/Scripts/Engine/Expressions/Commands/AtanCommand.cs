using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class AtanCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var value = parameters[0].Evaluate(variables);

				if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, value.Type, 0, VariableType.Number);

				return VariableValue.Create(Mathf.Atan(value.Number));
			}
			else if (parameters.Count == 2)
			{
				var y = parameters[0].Evaluate(variables);
				var x = parameters[1].Evaluate(variables);

				if (y.Type != VariableType.Integer && y.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, y.Type, 0, VariableType.Number);

				if (x.Type != VariableType.Integer && x.Type != VariableType.Number)
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, x.Type, 1, VariableType.Number);

				return VariableValue.Create(Mathf.Atan2(y.Number, x.Number));
			}
			else
			{
				throw new CommandEvaluationException(name, Command.WrongParameterRangeException, parameters.Count, "s", 1, 2);
			}
		}
	}
}
