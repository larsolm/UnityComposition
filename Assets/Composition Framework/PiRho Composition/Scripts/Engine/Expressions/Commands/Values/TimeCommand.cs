using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TimeCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return VariableValue.Create(Time.time);
			else
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
