using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector3IntCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);

				if (!xValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (@yValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (!zValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Int);

				return Variable.Vector3Int(new Vector3Int(xValue.AsInt, yValue.AsInt, zValue.AsInt));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
		}
	}
}
