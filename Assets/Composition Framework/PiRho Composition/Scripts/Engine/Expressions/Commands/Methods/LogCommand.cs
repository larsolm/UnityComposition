using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class LogCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			switch (parameters.Count)
			{
				case 1:
				{
					var value = parameters[0].Evaluate(variables);

					if (value.TryGetFloat(out var number))
						return VariableValue.Create(Mathf.Log(number, 10.0f));
					else
						throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Float);
				}
				case 2:
				{
					var value = parameters[0].Evaluate(variables);
					var power = parameters[1].Evaluate(variables);

					if (!value.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, 0, value.Type, VariableType.Float);

					if (!power.HasNumber)
						throw CommandEvaluationException.WrongParameterType(name, 1, power.Type, VariableType.Int, VariableType.Float);

					return VariableValue.Create(Mathf.Log(value.Number, power.Number));
				}
				default:
				{
					throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1, 2);
				}
			}
		}
	}
}
