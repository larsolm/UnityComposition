using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RandomCommand : Command
	{
		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			switch (parameters.Count)
			{
				case 0:
				{
					return VariableValue.Create(Random.value);
				}
				case 1:
				{
					var max = parameters[0].Evaluate(variables);

					ValidateType(ref max, name, 0);

					if (max.Type == VariableType.Integer)
						return VariableValue.Create(Random.Range(0, max.Integer));
					else
						return VariableValue.Create(Random.Range(0.0f, max.Number));
				}
				case 2:
				{
					var min = parameters[0].Evaluate(variables);
					var max = parameters[1].Evaluate(variables);

					ValidateType(ref min, name, 0);
					ValidateType(ref max, name, 1);

					if (min > max)
						throw new CommandEvaluationException(name, InvalidRangeException, min, max);

					if (min.Type == VariableType.Integer)
					{
						if (max.Type == VariableType.Integer)
							return VariableValue.Create(Random.Range(min.Integer, max.Integer));
						else
							return VariableValue.Create(Random.Range(min.Integer, max.Number));
					}
					else
					{
						if (max.Type == VariableType.Integer)
							return VariableValue.Create(Random.Range(min.Number, max.Integer));
						else
							return VariableValue.Create(Random.Range(min.Number, max.Number));
					}
				}
				default:
				{
					throw new CommandEvaluationException(name, TooManyParametersException, parameters.Count, "s", 2);
				}
			}
		}

		private void ValidateType(ref VariableValue value, string name, int index)
		{
			if (value.Type != VariableType.Integer && value.Type != VariableType.Number)
				throw new CommandEvaluationException(name, WrongParameterType2Exception, value.Type, index, VariableType.Integer, VariableType.Number);
		}
	}
}
