using System;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class AbsCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Integer: return VariableValue.Create(Math.Abs(result.Integer));
					case VariableType.Number: return VariableValue.Create(Math.Abs(result.Number));
					default: throw new CommandEvaluationException(name, Command.WrongParameterType2Exception, result.Type, 0, VariableType.Integer, VariableType.Number);
				}
			}
			else
			{
				throw new CommandEvaluationException(name, Command.WrongParameterCountException, parameters.Count, "s", 1);
			}
		}
	}
}
