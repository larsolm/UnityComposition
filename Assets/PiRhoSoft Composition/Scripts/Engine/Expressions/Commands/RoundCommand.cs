﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RoundCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				if (result.Type == VariableType.Number)
					return VariableValue.Create(Mathf.RoundToInt(result.Number));
				else
					throw new CommandEvaluationException(name, Command.WrongParameterType1Exception, result.Type, 0, VariableType.Number);
			}
			else
			{
				throw new CommandEvaluationException(name, Command.WrongParameterCountException, parameters.Count, "s", 1);
			}
		}
	}
}
