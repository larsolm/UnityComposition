using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class NegateOperator : PrefixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);

			switch (result.Type)
			{
				case VariableType.Int: return VariableValue.Create(-result.Int);
				case VariableType.Int2: return VariableValue.Create(new Vector2Int(-result.Int2.x, -result.Int2.y));
				case VariableType.Int3: return VariableValue.Create(new Vector3Int(-result.Int3.x, -result.Int3.y, -result.Int3.z));
				case VariableType.Float: return VariableValue.Create(-result.Float);
				case VariableType.Vector2: return VariableValue.Create(-result.Vector2);
				case VariableType.Vector3: return VariableValue.Create(-result.Vector3);
				case VariableType.Vector4: return VariableValue.Create(-result.Vector4);
			}

			throw TypeMismatch(result.Type, VariableType.Int, VariableType.Float, VariableType.Int2, VariableType.Vector2, VariableType.Int3, VariableType.Vector3, VariableType.Vector4);
		}
	}
}
