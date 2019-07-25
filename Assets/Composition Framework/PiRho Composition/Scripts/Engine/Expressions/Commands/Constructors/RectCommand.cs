using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RectCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.IsVector2)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector2);

				if (!sizeValue.IsVector2)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector2);

				return Variable.Rect(new Rect(positionValue.AsVector2, sizeValue.AsVector2));
			}
			else if (parameters.Count == 4)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var widthValue = parameters[2].Evaluate(variables);
				var heightValue = parameters[3].Evaluate(variables);

				if (!xValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);
				
				if (!yValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				if (!widthValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, widthValue.Type, VariableType.Float);

				if (!heightValue.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 3, heightValue.Type, VariableType.Float);

				return Variable.Rect(new Rect(xValue.AsFloat, yValue.AsFloat, widthValue.AsFloat, heightValue.AsFloat));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2, 4);
		}
	}
}
