namespace PiRhoSoft.CompositionEngine
{
	public class SubtractOperator : InfixOperation
	{
		public static VariableValue Subtract(string symbol, ref VariableValue left, ref VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Int:
				{
					switch (right.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Int - right.Int);
						case VariableType.Float: return VariableValue.Create(left.Int - right.Float);
					}

					break;
				}
				case VariableType.Int2:
				{
					switch (right.Type)
					{
						case VariableType.Int2: return VariableValue.Create(left.Int2 - right.Int2);
						case VariableType.Vector2: return VariableValue.Create(left.Int2 - right.Vector2);
					}

					break;
				}
				case VariableType.Int3:
				{
					if (right.TryGetInt3(out var int3))
						return VariableValue.Create(left.Int3 - int3);
					else if (right.TryGetVector3(out var vector3))
						return VariableValue.Create(left.Int3 - vector3);

					break;
				}
				case VariableType.Float:
				{
					switch (right.Type)
					{
						case VariableType.Int: return VariableValue.Create(left.Float - right.Int);
						case VariableType.Float: return VariableValue.Create(left.Float - right.Float);
					}

					break;
				}
				case VariableType.Vector2:
				{
					if (right.TryGetVector2(out var vector2))
						return VariableValue.Create(left.Vector2 - right.Vector2);

					break;
				}
				case VariableType.Vector3:
				{
					if (right.TryGetVector3(out var vector3))
						return VariableValue.Create(left.Vector3 - vector3);

					break;
				}
				case VariableType.Vector4:
				{
					if (right.TryGetVector4(out var vector4))
						return VariableValue.Create(left.Vector4 - vector4);

					break;
				}
			}

			throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Int, VariableType.Float, VariableType.Int2, VariableType.Vector2, VariableType.Int3, VariableType.Vector3, VariableType.Vector4);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Subtract(Symbol, ref left, ref right);
		}
	}
}
