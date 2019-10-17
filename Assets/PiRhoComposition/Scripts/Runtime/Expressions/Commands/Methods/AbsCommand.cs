using System;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	internal class AbsCommand : ICommand
	{
		public Variable Evaluate(IVariableMap variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return Variable.Int(Math.Abs(result.AsInt));
					case VariableType.Float: return Variable.Float(Math.Abs(result.AsFloat));
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
