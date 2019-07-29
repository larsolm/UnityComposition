using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class BoundsCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.IsVector3)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector3);

				if (!sizeValue.IsVector3)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector3);

				return Variable.Bounds(new Bounds(positionValue.AsVector3, sizeValue.AsVector3));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
