using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RectIntCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 2)
			{
				var positionValue = parameters[0].Evaluate(variables);
				var sizeValue = parameters[1].Evaluate(variables);

				if (positionValue.Type != VariableType.Int2)
					throw CommandEvaluationException.WrongParameterType(name, 0, positionValue.Type, VariableType.Int2);

				if (sizeValue.Type != VariableType.Int2)
					throw CommandEvaluationException.WrongParameterType(name, 1, sizeValue.Type, VariableType.Int2);

				return VariableValue.Create(new RectInt(positionValue.Int2, sizeValue.Int2));
			}
			else if (parameters.Count == 4)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var widthValue = parameters[2].Evaluate(variables);
				var heightValue = parameters[3].Evaluate(variables);

				if (xValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (yValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (widthValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 2, widthValue.Type, VariableType.Int);

				if (heightValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 3, heightValue.Type, VariableType.Int);

				return VariableValue.Create(new RectInt(xValue.Int, yValue.Int, widthValue.Int, heightValue.Int));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 2);
		}
	}
}
