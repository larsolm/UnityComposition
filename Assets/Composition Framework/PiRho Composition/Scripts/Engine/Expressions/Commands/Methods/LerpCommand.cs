using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class LerpCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var a = parameters[0].Evaluate(variables);
				var b = parameters[1].Evaluate(variables);
				var t = parameters[2].Evaluate(variables);

				if (!t.IsFloat)
					throw CommandEvaluationException.WrongParameterType(name, 2, t.Type, VariableType.Float);

				if (a.IsFloat && b.IsFloat)
					return Variable.Float(Mathf.Lerp(a.AsFloat, b.AsFloat, t.AsFloat));
				else if (a.IsVector2 && b.IsVector2)
					return Variable.Vector2(Vector2.Lerp(a.AsVector2, b.AsVector2, t.AsFloat));
				else if (a.IsVector3 && b.IsVector3)
					return Variable.Vector3(Vector3.Lerp(a.AsVector3, b.AsVector3, t.AsFloat));
				else if (a.IsVector4 && b.IsVector4)
					return Variable.Vector4(Vector4.Lerp(a.AsVector4, b.AsVector4, t.AsFloat));
				else if (a.Type == VariableType.Quaternion && b.Type == VariableType.Quaternion)
					return Variable.Quaternion(Quaternion.Lerp(a.AsQuaternion, b.AsQuaternion, t.AsFloat));
				else if (a.Type == VariableType.Color && b.Type == VariableType.Color)
					return Variable.Color(Color.Lerp(a.AsColor, b.AsColor, t.AsFloat));

				if (a.IsFloat || a.IsVector4 || a.IsQuaternion || a.IsColor)
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
