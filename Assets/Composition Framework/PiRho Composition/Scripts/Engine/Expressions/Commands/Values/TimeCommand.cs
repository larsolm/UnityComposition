﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TimeCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return Variable.Float(Time.time);
			else
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
