﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class UnscaledTimeCommand : ICommand
	{
		public Variable Evaluate(IVariableMap variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return Variable.Float(Time.unscaledTime);
			else
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
