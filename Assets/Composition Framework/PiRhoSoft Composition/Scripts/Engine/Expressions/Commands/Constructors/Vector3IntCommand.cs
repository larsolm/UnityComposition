﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class Vector3IntCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var xValue = parameters[0].Evaluate(variables);
				var yValue = parameters[1].Evaluate(variables);
				var zValue = parameters[2].Evaluate(variables);

				if (xValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, xValue.Type, VariableType.Int);

				if (yValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 1, yValue.Type, VariableType.Int);

				if (zValue.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 2, zValue.Type, VariableType.Int);

				return VariableValue.Create(new Vector3Int(xValue.Int, yValue.Int, zValue.Int));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
		}
	}
}
