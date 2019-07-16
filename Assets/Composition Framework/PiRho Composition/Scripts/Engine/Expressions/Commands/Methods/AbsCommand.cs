using System;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	internal class AbsCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return VariableValue.Create(Math.Abs(result.Int));
					case VariableType.Float: return VariableValue.Create(Math.Abs(result.Float));
				}

				throw CommandEvaluationException.WrongParameterType(name, 0, result.Type, VariableType.Int, VariableType.Float);
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1);
			}
		}
	}
}
