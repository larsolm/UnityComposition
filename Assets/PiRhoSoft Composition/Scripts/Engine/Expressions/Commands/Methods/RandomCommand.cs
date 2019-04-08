﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RandomCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
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

					if (max.Type == VariableType.Int)
						return VariableValue.Create(Random.Range(0, max.Int));
					else if (max.Type == VariableType.Float)
						return VariableValue.Create(Random.Range(0.0f, max.Float));
					else
						throw CommandEvaluationException.WrongParameterType(name, 0, max.Type, VariableType.Int, VariableType.Float);
				}
				case 2:
				{
					var min = parameters[0].Evaluate(variables);
					var max = parameters[1].Evaluate(variables);

					if (min.Type == VariableType.Int)
					{
						if (max.Type == VariableType.Int)
							return VariableValue.Create(Random.Range(min.Int, max.Int));
						else if (max.Type == VariableType.Float)
							return VariableValue.Create(Random.Range(min.Int, max.Float));
						else
							throw CommandEvaluationException.WrongParameterType(name, 1, max.Type, VariableType.Int, VariableType.Float);
					}
					else if (min.Type == VariableType.Float)
					{
						if (max.Type == VariableType.Int)
							return VariableValue.Create(Random.Range(min.Float, max.Int));
						else if (max.Type == VariableType.Float)
							return VariableValue.Create(Random.Range(min.Float, max.Float));
						else
							throw CommandEvaluationException.WrongParameterType(name, 1, max.Type, VariableType.Int, VariableType.Float);
					}
					else
					{
						throw CommandEvaluationException.WrongParameterType(name, 0, min.Type, VariableType.Int, VariableType.Float);
					}
				}
				default:
				{
					throw CommandEvaluationException.TooManyParameters(name, parameters.Count, 2);
				}
			}
		}
	}
}