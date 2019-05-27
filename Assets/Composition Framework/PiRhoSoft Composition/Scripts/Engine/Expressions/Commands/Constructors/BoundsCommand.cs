using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class BoundsCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.HasNumber3)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector3);

				if (!sizeValue.HasNumber3)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector3);

				return VariableValue.Create(new Bounds(positionValue.Number3, sizeValue.Number3));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
