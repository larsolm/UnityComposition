using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Int3VariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector3Int());
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Int3);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int3.x);
			writer.Write(value.Int3.y);
			writer.Write(value.Int3.z);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();

			return VariableValue.Create(new Vector3Int(x, y, z));
		}

		protected override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt3(out var i3))
				return VariableValue.Create(left.Int3 + i3);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt3(out var i3))
				return VariableValue.Create(left.Int3 - i3);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int3 * i);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(new Vector3Int(left.Int2.x / i, left.Int2.y / i, left.Int3.z / i));
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(new Vector3(left.Int2.x / f, left.Int2.y / f, left.Int3.z / f));
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(new Vector3Int(-value.Int3.x, -value.Int3.y, -value.Int3.z));
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Int3.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Int3.y);
				else if (lookup.String == "z") return VariableValue.Create(owner.Int3.z);
			}

			return VariableValue.Empty;
		}

		protected override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "x")
					{
						owner = VariableValue.Create(new Vector3Int(value.Int, owner.Int3.y, owner.Int3.z));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Vector3Int(owner.Int3.x, value.Int, owner.Int3.z));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "z")
					{
						owner = VariableValue.Create(new Vector3Int(owner.Int3.x, owner.Int3.y, value.Int));
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

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt3(out var i3))
				return left.Int3 == i3;
			else
				return null;
		}
	}
}
