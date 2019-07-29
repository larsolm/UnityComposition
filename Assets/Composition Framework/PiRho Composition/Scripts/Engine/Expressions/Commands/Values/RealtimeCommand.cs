﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RealtimeCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return Variable.Float(Time.realtimeSinceStartup);
			else
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
