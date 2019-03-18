namespace PiRhoSoft.CompositionEngine
{
	public abstract class ComparisonOperation : InfixOperation
	{
		// Valid comparisons follow the same casting rules as laid out in the Casting region of the VariableValue
		// definition with the addition that VariableType Empty compares equal to null objects. Comparison results
		// follow the same rules as the .net CompareTo method.

		protected bool Equals(VariableValue left, VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Empty:
				{
					if (right.Type == VariableType.Empty) return true;
					else if (right.HasRect) return right.IsNull;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Empty, VariableType.Object, VariableType.Store, VariableType.Other);
				}
				case VariableType.Bool:
				{
					if (right.Type == VariableType.Bool) return left.Bool == right.Bool;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Bool);
				}
				case VariableType.Int:
				{
					if (right.Type == VariableType.Int) return left.Int == right.Int;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Int);
				}
				case VariableType.Float:
				{
					if (right.TryGetFloat(out var number)) return left.Float == right.Number;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Float);
				}
				case VariableType.Int2:
				{
					if (right.TryGetInt2(out var int2)) return left.Int2 == int2;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Int2);
				}
				case VariableType.Int3:
				{
					if (right.TryGetInt3(out var int3)) return left.Int3 == int3;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Int3);
				}
				case VariableType.IntRect:
				{
					if (right.TryGetIntRect(out var intRect)) return left.IntRect.x == intRect.x && left.IntRect.y == intRect.y && left.IntRect.width == intRect.width && left.IntRect.height == intRect.height;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.IntRect);
				}
				case VariableType.IntBounds:
				{
					if (right.TryGetIntBounds(out var intBounds)) return left.IntBounds == intBounds;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.IntBounds);
				}
				case VariableType.Vector2:
				{
					if (right.TryGetVector2(out var vector2)) return left.Vector2 == vector2;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Vector2);
				}
				case VariableType.Vector3:
				{
					if (right.TryGetVector3(out var vector3)) return left.Vector3 == vector3;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Vector3);
				}
				case VariableType.Vector4:
				{
					if (right.TryGetVector4(out var vector4)) return left.Vector4 == vector4;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Vector4);
				}
				case VariableType.Quaternion:
				{
					if (right.TryGetQuaternion(out var quaternion)) return left.Quaternion == quaternion;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Quaternion);
				}
				case VariableType.Rect:
				{
					if (right.TryGetRect(out var rect)) return left.Rect == rect;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Rect);
				}
				case VariableType.Bounds:
				{
					if (right.TryGetBounds(out var bounds)) return left.Bounds == bounds;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Bounds);
				}
				case VariableType.Color:
				{
					if (right.TryGetColor(out var color)) return left.Color == color;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Color);
				}
				case VariableType.String:
				{
					if (right.TryGetString(out var str)) return left.String == str;
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.String);
				}
				default: // Object, Store, Other
				{
					return left.Reference.Equals(right.Reference);
				}
			}
		}

		protected int Compare(VariableValue left, VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Int:
				{
					if (right.Type == VariableType.Int) return left.Int.CompareTo(right.Int);
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Int);
				}
				case VariableType.Float:
				{
					if (right.TryGetFloat(out var number)) return left.Float.CompareTo(right.Number);
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.Float);
				}
				case VariableType.String:
				{
					if (right.Type == VariableType.String) return left.String.CompareTo(right.String);
					else throw ComparisonMismatch(left.Type, right.Type, VariableType.String);
				}
			}

			throw TypeMismatch(left.Type, right.Type, VariableType.Int, VariableType.Float, VariableType.String);
		}

		private ExpressionEvaluationException ComparisonMismatch(VariableType left, VariableType right, VariableType expected) => ExpressionEvaluationException.ComparisonTypeMismatch(Symbol, left, right, expected);
		private ExpressionEvaluationException ComparisonMismatch(VariableType left, VariableType right, VariableType expected1, VariableType expected2) => ExpressionEvaluationException.ComparisonTypeMismatch(Symbol, left, right, expected1, expected2);
		private ExpressionEvaluationException ComparisonMismatch(VariableType left, VariableType right, params VariableType[] expected) => ExpressionEvaluationException.ComparisonTypeMismatch(Symbol, left, right, expected);
	}
}
