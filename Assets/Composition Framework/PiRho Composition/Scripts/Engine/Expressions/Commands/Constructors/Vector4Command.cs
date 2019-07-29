using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector4Command : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 4)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);
				var wValue = parameters[3].Evaluate(variables);

				if (!xValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);

				if (!yValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				if (!zValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Float);

				if (!wValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 3, wValue.Type, VariableType.Float);

				return Variable.Vector4(new Vector4(xValue.AsFloat, yValue.AsFloat, zValue.AsFloat, wValue.AsFloat));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 4);
		}
	}
}
