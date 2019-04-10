using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RectCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (!positionValue.HasNumber2)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Vector2);

				if (!sizeValue.HasNumber2)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Vector2);

				return VariableValue.Create(new Rect(positionValue.Number2, sizeValue.Number2));
			}
			else if (parameters.Count == 4)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var widthValue = parameters[2].Evaluate(variables);
				var heightValue = parameters[3].Evaluate(variables);

				if (xValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Float);
				
				if (yValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Float);

				if (widthValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, widthValue.Type, VariableType.Float);

				if (heightValue.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 3, heightValue.Type, VariableType.Float);

				return VariableValue.Create(new Rect(xValue.Number, yValue.Number, widthValue.Number, heightValue.Number));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2, 4);
		}
	}
}
