using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class BoundsIntCommand : ICommand
	{
		public Variable Evaluate(IVariableMap variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.IsVector3Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector3Int);

				if (!sizeValue.IsVector3Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector3Int);

				return Variable.BoundsInt(new BoundsInt(positionValue.AsVector3Int, sizeValue.AsVector3Int));
			}
			else if (parameters.Count == 6)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);
				var widthValue = parameters[3].Evaluate(variables);
				var heightValue = parameters[4].Evaluate(variables);
				var depthValue = parameters[4].Evaluate(variables);

				if (!xValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (!yValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (!zValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Int);

				if (!widthValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 3, widthValue.Type, VariableType.Int);

				if (!heightValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 4, heightValue.Type, VariableType.Int);

				if (!depthValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 5, depthValue.Type, VariableType.Int);

				return Variable.BoundsInt(new BoundsInt(xValue.AsInt, yValue.AsInt, zValue.AsInt, widthValue.AsInt, heightValue.AsInt, depthValue.AsInt));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2, 6);
		}
	}
}
