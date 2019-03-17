using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class LerpCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var a = parameters[0].Evaluate(variables);
				var b = parameters[1].Evaluate(variables);
				var t = parameters[2].Evaluate(variables);

				if (!t.HasNumber)
					throw CommandEvaluationException.WrongParameterType(name, 2, t.Type, VariableType.Float);

				if (a.HasNumber && b.HasNumber)
					return VariableValue.Create(Mathf.Lerp(a.Number, b.Number, t.Number));
				else if (a.HasNumber2 && b.HasNumber2)
					return VariableValue.Create(Vector2.Lerp(a.Number2, b.Number2, t.Number));
				else if (a.HasNumber3 && b.HasNumber3)
					return VariableValue.Create(Vector3.Lerp(a.Number3, b.Number3, t.Number));
				else if (a.HasNumber4 && b.HasNumber4)
					return VariableValue.Create(Vector4.Lerp(a.Number4, b.Number4, t.Number));
				else if (a.Type == VariableType.Quaternion && b.Type == VariableType.Quaternion)
					return VariableValue.Create(Quaternion.Lerp(a.Quaternion, b.Quaternion, t.Number));
				else if (a.Type == VariableType.Color && b.Type == VariableType.Color)
					return VariableValue.Create(Color.Lerp(a.Color, b.Color, t.Number));

				if (a.HasNumber || a.HasNumber4 || a.Type == VariableType.Quaternion || a.Type == VariableType.Color)
					throw CommandEvaluationException.WrongParameterType(name, 1, b.Type, a.Type);
				else
					throw CommandEvaluationException.WrongParameterType(name, 0, a.Type, VariableType.Float, VariableType.Vector2, VariableType.Vector3, VariableType.Vector4, VariableType.Quaternion, VariableType.Color);
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 3);
			}
		}
	}
}
