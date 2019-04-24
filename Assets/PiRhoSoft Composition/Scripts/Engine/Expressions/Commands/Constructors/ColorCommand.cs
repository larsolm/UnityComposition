using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class ColorCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var rValue = parameters[0].Evaluate(variables);
				var gValue = parameters[1].Evaluate(variables);
				var bValue = parameters[2].Evaluate(variables);

				if (!rValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, rValue.Type, VariableType.Float);

				if (!gValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, gValue.Type, VariableType.Float);

				if (!bValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, bValue.Type, VariableType.Float);

				return VariableValue.Create(new Color(rValue.Number, gValue.Number, bValue.Number));
			}
			else if (parameters.Count == 4)
			{
				var rValue = parameters[0].Evaluate(variables);
				var gValue = parameters[1].Evaluate(variables);
				var bValue = parameters[2].Evaluate(variables);
				var aValue = parameters[3].Evaluate(variables);

				if (!rValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, rValue.Type, VariableType.Float);

				if (!gValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, bValue.Type, VariableType.Float);

				if (!bValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, gValue.Type, VariableType.Float);

				if (!aValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 3, aValue.Type, VariableType.Float);

				return VariableValue.Create(new Vector4(rValue.Number, gValue.Number, bValue.Number, aValue.Number));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3, 4);
		}
	}
}
