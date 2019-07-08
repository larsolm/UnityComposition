using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class BoundsIntCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (positionValue.Type != VariableType.Int3)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Int3);

				if (sizeValue.Type != VariableType.Int3)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Int3);

				return VariableValue.Create(new BoundsInt(positionValue.Int3, sizeValue.Int3));
			}
			else if (parameters.Count == 6)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);
				var widthValue = parameters[3].Evaluate(variables);
				var heightValue = parameters[4].Evaluate(variables);
				var depthValue = parameters[4].Evaluate(variables);

				if (xValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (yValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (zValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Int);

				if (widthValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 3, widthValue.Type, VariableType.Int);

				if (heightValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 4, heightValue.Type, VariableType.Int);

				if (depthValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 5, depthValue.Type, VariableType.Int);

				return VariableValue.Create(new BoundsInt(xValue.Int, yValue.Int, zValue.Int, widthValue.Int, heightValue.Int, depthValue.Int));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2, 6);
		}
	}
}
