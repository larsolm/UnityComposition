using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class AtanCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			switch (parameters.Count)
			{
				case 1:
				{
					var value = parameters[0].Evaluate(variables);

					if (value.TryGetFloat(out var number))
						return VariableValue.Create(Mathf.Atan(number));
					else if (value.TryGetVector2(out var vector))
						return VariableValue.Create(Mathf.Atan2(vector.y, vector.x));
					else
						throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Float, VariableType.Vector2);
				}
				case 2:
				{
					var y = parameters[0].Evaluate(variables);
					var x = parameters[1].Evaluate(variables);

					if (!y.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, 0, y.Type, VariableType.Float);

					if (!x.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, 1, x.Type, VariableType.Float);

					return VariableValue.Create(Mathf.Atan2(y.Number, x.Number));
				}
				default:
				{
					throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1, 2);
				}
			}
		}
	}
}
