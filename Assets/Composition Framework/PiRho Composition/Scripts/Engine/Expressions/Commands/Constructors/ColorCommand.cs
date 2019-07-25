using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class ColorCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var rValue = parameters[0].Evaluate(variables);
				var gValue = parameters[1].Evaluate(variables);
				var bValue = parameters[2].Evaluate(variables);

				if (!rValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, rValue.Type, VariableType.Float);

				if (!gValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, gValue.Type, VariableType.Float);

				if (!bValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, bValue.Type, VariableType.Float);

				return Variable.Color(new Color(rValue.AsFloat, gValue.AsFloat, bValue.AsFloat));
			}
			else if (parameters.Count == 4)
			{
				var rValue = parameters[0].Evaluate(variables);
				var gValue = parameters[1].Evaluate(variables);
				var bValue = parameters[2].Evaluate(variables);
				var aValue = parameters[3].Evaluate(variables);

				if (!rValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, rValue.Type, VariableType.Float);

				if (!gValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, bValue.Type, VariableType.Float);

				if (!bValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, gValue.Type, VariableType.Float);

				if (!aValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 3, aValue.Type, VariableType.Float);

				return Variable.Color(new Color(rValue.AsFloat, gValue.AsFloat, bValue.AsFloat, aValue.AsFloat));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3, 4);
		}
	}
}
