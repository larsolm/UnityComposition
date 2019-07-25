using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RectIntCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.IsVector2Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector2Int);

				if (!sizeValue.IsVector2Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector2Int);

				return Variable.RectInt(new RectInt(positionValue.AsVector2Int, sizeValue.AsVector2Int));
			}
			else if (parameters.Count == 4)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var widthValue = parameters[2].Evaluate(variables);
				var heightValue = parameters[3].Evaluate(variables);

				if (!xValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (!yValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (!widthValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 2, widthValue.Type, VariableType.Int);

				if (!heightValue.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 3, heightValue.Type, VariableType.Int);

				return Variable.RectInt(new RectInt(xValue.AsInt, yValue.AsInt, widthValue.AsInt, heightValue.AsInt));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2, 4);
		}
	}
}
