using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector4Command : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);
				var wValue = parameters[3].Evaluate(variables);

				if (!xValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);

				if (!yValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				if (!zValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Float);

				if (!wValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 3, wValue.Type, VariableType.Float);

				return VariableValue.Create(new Vector4(xValue.Number, yValue.Number, zValue.Number, wValue.Number));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 4);
		}
	}
}
