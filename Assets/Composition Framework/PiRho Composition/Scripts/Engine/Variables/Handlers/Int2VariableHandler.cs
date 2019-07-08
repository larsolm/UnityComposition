using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class Int2VariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector2Int());
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Int2);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int2.x);
			writer.Write(value.Int2.y);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();

			return VariableValue.Create(new Vector2Int(x, y));
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt2(out var i2))
				return VariableValue.Create(left.Int2 + i2);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt2(out var i2))
				return VariableValue.Create(left.Int2 - i2);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int2 * i);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(new Vector2Int(left.Int2.x / i, left.Int2.y / i));
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(new Vector2(left.Int2.x / f, left.Int2.y / f));
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(new Vector2Int(-value.Int2.x, -value.Int2.y));
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Int2.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Int2.y);
			}

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "x")
					{
						owner = VariableValue.Create(new Vector2Int(value.Int, owner.Int2.y));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Vector2Int(owner.Int2.x, value.Int));
						return SetVariableResult.Success;
					}
				}

				return SetVariableResult.NotFound;
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt2(out var int2))
				return left.Int2 == int2;
			else
				return null;
		}
	}
}
