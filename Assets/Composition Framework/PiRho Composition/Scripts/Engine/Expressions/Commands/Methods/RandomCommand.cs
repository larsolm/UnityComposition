using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RandomCommand : ICommand
	{
		public Variable Evaluate(IVariableCollection variables, string name, List<Operation> parameters)
		{
			switch (parameters.Count)
			{
				case 0:
				{
					return Variable.Float(Random.value);
				}
				case 1:
				{
					var value = parameters[0].Evaluate(variables);

					switch (value.Type)
					{
						case VariableType.Int: return Variable.Int(Random.Range(0, value.AsInt));
						case VariableType.Float: return Variable.Float(Random.Range(0.0f, value.AsFloat));
						case VariableType.List: return value.AsList.GetVariable(Random.Range(0, value.AsList.VariableCount));
						default: throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Int, VariableType.Float, VariableType.List);
					}
				}
				case 2:
				{
					var min = parameters[0].Evaluate(variables);
					var max = parameters[1].Evaluate(variables);

					if (min.Type == VariableType.Int)
					{
						if (max.Type == VariableType.Int)
							return Variable.Int(Random.Range(min.AsInt, max.AsInt));
						else if (max.Type == VariableType.Float)
							return Variable.Float(Random.Range(min.AsInt, max.AsFloat));
						else
							throw CommandEvaluationException.WrongParameterType(name, 1, max.Type, VariableType.Int, VariableType.Float);
					}
					else if (min.Type == VariableType.Float)
					{
						if (max.Type == VariableType.Int)
							return Variable.Float(Random.Range(min.AsFloat, max.AsInt));
						else if (max.Type == VariableType.Float)
							return Variable.Float(Random.Range(min.AsFloat, max.AsFloat));
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
