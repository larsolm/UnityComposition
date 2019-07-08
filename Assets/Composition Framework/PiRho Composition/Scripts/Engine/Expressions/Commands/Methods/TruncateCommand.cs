using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class TruncateCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 1)
			{
				var result = parameters[0].Evaluate(variables);

				switch (result.Type)
				{
					case VariableType.Int: return result;
					case VariableType.Int2: return result;
					case VariableType.Int3: return result;
					case VariableType.IntRect: return result;
					case VariableType.IntBounds: return result;
					case VariableType.Float: return VariableValue.Create((int)result.Float);
					case VariableType.Vector2: return VariableValue.Create(new Vector2Int((int)result.Vector2.x, (int)result.Vector2.y));
					case VariableType.Vector3: return VariableValue.Create(new Vector3Int((int)result.Vector3.x, (int)result.Vector3.y, (int)result.Vector3.z));
					case VariableType.Rect: return VariableValue.Create(new RectInt((int)result.Rect.x, (int)result.Rect.y, (int)result.Rect.width, (int)result.Rect.height));
					case VariableType.Bounds: return VariableValue.Create(new BoundsInt((int)result.Bounds.min.x, (int)result.Bounds.min.y, (int)result.Bounds.min.z, (int)result.Bounds.size.x, (int)result.Bounds.size.y, (int)result.Bounds.size.z));
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
