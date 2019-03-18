using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class MultiplyOperator : InfixOperation
	{
		public static VariableValue Multiply(string symbol, ref VariableValue left, ref VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Int:
				{
					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int * right.Int);
						case VariableType.Int2: return VariableValue.Create(left.Int2 * right.Int);
						case VariableType.Int3: return VariableValue.Create(left.Int3 * right.Int);
						case VariableType.Float: return VariableValue.Create(left.Float * right.Int);
						case VariableType.Vector2: return VariableValue.Create(left.Vector2 * right.Int);
						case VariableType.Vector3: return VariableValue.Create(left.Vector3 * right.Int);
						case VariableType.Vector4: return VariableValue.Create(left.Vector4 * right.Int);
					}

					break;
				}
				case VariableType.Float:
				{
					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int * right.Float);
						case VariableType.Int2: return VariableValue.Create(new Vector2(left.Int2.x * right.Float, left.Int2.y * right.Float));
						case VariableType.Int3: return VariableValue.Create(new Vector3(left.Int3.x * right.Float, left.Int3.y * right.Float, left.Int3.z * right.Float));
						case VariableType.Float: return VariableValue.Create(left.Float * right.Float);
						case VariableType.Vector2: return VariableValue.Create(left.Vector2 * right.Float);
						case VariableType.Vector3: return VariableValue.Create(left.Vector3 * right.Float);
						case VariableType.Vector4: return VariableValue.Create(left.Vector4 * right.Float);
					}

					break;
				}
			}

			throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Int, VariableType.Float, VariableType.Int2, VariableType.Vector2, VariableType.Int3, VariableType.Vector3, VariableType.Vector4);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Multiply(Symbol, ref left, ref right);
		}
	}
}
