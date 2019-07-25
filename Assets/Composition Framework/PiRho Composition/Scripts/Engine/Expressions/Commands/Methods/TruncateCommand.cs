using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TruncateCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return result;
					case VariableType.Vector2Int: return result;
					case VariableType.Vector3Int: return result;
					case VariableType.RectInt: return result;
					case VariableType.BoundsInt: return result;
					case VariableType.Float: return Variable.Int((int)result.AsFloat);
					case VariableType.Vector2: return Variable.Vector2Int(new Vector2Int((int)result.AsVector2.x, (int)result.AsVector2.y));
					case VariableType.Vector3: return Variable.Vector3Int(new Vector3Int((int)result.AsVector3.x, (int)result.AsVector3.y, (int)result.AsVector3.z));
					case VariableType.Rect: return Variable.RectInt(new RectInt((int)result.AsRect.x, (int)result.AsRect.y, (int)result.AsRect.width, (int)result.AsRect.height));
					case VariableType.Bounds: return Variable.BoundsInt(new BoundsInt((int)result.AsBounds.min.x, (int)result.AsBounds.min.y, (int)result.AsBounds.min.z, (int)result.AsBounds.size.x, (int)result.AsBounds.size.y, (int)result.AsBounds.size.z));
				}

				throw CommandEvaluationException.WrongParameterType(name, 0, result.Type, VariableType.Float, VariableType.Vector2, VariableType.Vector3, VariableType.Rect, VariableType.Bounds);
			}
			else
			{
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 1);
			}
		}
	}
}
