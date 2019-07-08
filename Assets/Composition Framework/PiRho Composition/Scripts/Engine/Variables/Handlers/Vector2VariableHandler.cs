using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class Vector2VariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector2());
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Vector2);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Vector2.x);
			writer.Write(value.Vector2.y);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();

			return VariableValue.Create(new Vector2(x, y));
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector2(out var v2))
				return VariableValue.Create(left.Vector2 + v2);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector2(out var v2))
				return VariableValue.Create(left.Vector2 - v2);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Vector2 * f);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Vector2 / number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(new Vector2(-value.Vector2.x, -value.Vector2.y));
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Vector2.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Vector2.y);
			}

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var f))
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "x")
					{
						owner = VariableValue.Create(new Vector2(f, owner.Vector2.y));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Vector2(owner.Vector2.x, f));
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
			if (right.TryGetVector2(out var int2))
				return left.Vector2 == int2;
			else
				return null;
		}
	}
}
