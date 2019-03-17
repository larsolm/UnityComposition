using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class DivideOperator : InfixOperation
	{
		public static VariableValue Divide(string symbol, ref VariableValue left, ref VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Int:
				{
					if (right.Int == 0)
						throw ExpressionEvaluationException.DivideByZero(symbol);

					var denominator = 1.0f / right.Int;

					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int * denominator);
						case VariableType.Int2: return VariableValue.Create(new Vector2(left.Int2.x * denominator, left.Int2.y * denominator));
						case VariableType.Int3: return VariableValue.Create(new Vector3(left.Int3.x * denominator, left.Int3.y * denominator, left.Int3.z * denominator));
						case VariableType.Float: return VariableValue.Create(left.Float * denominator);
						case VariableType.Vector2: return VariableValue.Create(left.Vector2 * denominator);
						case VariableType.Vector3: return VariableValue.Create(left.Vector3 * denominator);
						case VariableType.Vector4: return VariableValue.Create(left.Vector4 * denominator);
					}

					break;
				}
				case VariableType.Float:
				{
					var denominator = 1.0f / right.Float;

					switch (left.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int * denominator);
						case VariableType.Int2: return VariableValue.Create(new Vector2(left.Int2.x * denominator, left.Int2.y * denominator));
						case VariableType.Int3: return VariableValue.Create(new Vector3(left.Int3.x * denominator, left.Int3.y * denominator, left.Int3.z * denominator));
						case VariableType.Float: return VariableValue.Create(left.Float * denominator);
						case VariableType.Vector2: return VariableValue.Create(left.Vector2 * denominator);
						case VariableType.Vector3: return VariableValue.Create(left.Vector3 * denominator);
						case VariableType.Vector4: return VariableValue.Create(left.Vector4 * denominator);
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

			return Divide(Symbol, ref left, ref right);
		}
	}
}
