using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class UnscaledTimeCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count != 0)
				throw new CommandEvaluationException(name, Command.TooManyParametersException, parameters.Count, parameters.Count == 1 ? "" : "s", 0);

			return VariableValue.Create(Time.unscaledTime);
		}
	}
}
