using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntBoundsVariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(new BoundsInt());
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.IntBounds.min.x);
			writer.Write(value.IntBounds.min.y);
			writer.Write(value.IntBounds.min.z);
			writer.Write(value.IntBounds.size.x);
			writer.Write(value.IntBounds.size.y);
			writer.Write(value.IntBounds.size.z);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();
			var d = reader.ReadInt32();

			value = VariableValue.Create(new BoundsInt(x, y, z, w, h, d));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			switch (lookup)
			{
				case "x": return VariableValue.Create(owner.IntBounds.min.x);
				case "y": return VariableValue.Create(owner.IntBounds.min.y);
				case "z": return VariableValue.Create(owner.IntBounds.min.z);
				case "w": return VariableValue.Create(owner.IntBounds.size.x);
				case "h": return VariableValue.Create(owner.IntBounds.size.y);
				case "d": return VariableValue.Create(owner.IntBounds.size.z);
				default: return VariableValue.Empty;
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				switch (lookup)
				{
					case "x":
					{
						owner = VariableValue.Create(new BoundsInt(new Vector3Int(value.Int, owner.IntBounds.min.y, owner.IntBounds.min.z), owner.IntBounds.size));
						return SetVariableResult.Success;
					}
					case "y":
					{
						owner = VariableValue.Create(new BoundsInt(new Vector3Int(owner.IntBounds.min.x, value.Int, owner.IntBounds.min.z), owner.IntBounds.size));
						return SetVariableResult.Success;
					}
					case "z":
					{
						owner = VariableValue.Create(new BoundsInt(new Vector3Int(owner.IntBounds.min.x, owner.IntBounds.min.y, value.Int), owner.IntBounds.size));
						return SetVariableResult.Success;
					}
					case "w":
					{
						owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(value.Int, owner.IntBounds.size.y, owner.IntBounds.size.z)));
						return SetVariableResult.Success;
					}
					case "h":
					{
						owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(owner.IntBounds.size.x, value.Int, owner.IntBounds.size.z)));
						return SetVariableResult.Success;
					}
					case "d":
					{
						owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(owner.IntBounds.size.x, owner.IntBounds.size.y, value.Int)));
						return SetVariableResult.Success;
					}
					default:
					{
						return SetVariableResult.NotFound;
					}
				}
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}

		public override bool? IsEqual(VariableValue left, VariableValue right)
		{
			if (right.TryGetIntBounds(out var intBounds))
				return left.IntBounds == intBounds;
			else
				return null;
		}
	}
}
