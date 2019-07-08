using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class IntBoundsVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(new BoundsInt());
		protected internal override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.IntBounds);

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.IntBounds.min.x);
			writer.Write(value.IntBounds.min.y);
			writer.Write(value.IntBounds.min.z);
			writer.Write(value.IntBounds.size.x);
			writer.Write(value.IntBounds.size.y);
			writer.Write(value.IntBounds.size.z);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();
			var d = reader.ReadInt32();

			return VariableValue.Create(new BoundsInt(new Vector3Int(x, y, z), new Vector3Int(w, h, d)));
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				switch (lookup.String)
				{
					case "x": return VariableValue.Create(owner.IntBounds.min.x);
					case "y": return VariableValue.Create(owner.IntBounds.min.y);
					case "z": return VariableValue.Create(owner.IntBounds.min.z);
					case "w": return VariableValue.Create(owner.IntBounds.size.x);
					case "h": return VariableValue.Create(owner.IntBounds.size.y);
					case "d": return VariableValue.Create(owner.IntBounds.size.z);
				}
			}

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetInt(out var number))
			{
				if (lookup.Type == VariableType.String)
				{
					switch (lookup.String)
					{
						case "x":
						{
							owner = VariableValue.Create(new BoundsInt(new Vector3Int(number, owner.IntBounds.min.y, owner.IntBounds.min.z), owner.IntBounds.size));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = VariableValue.Create(new BoundsInt(new Vector3Int(owner.IntBounds.min.x, number, owner.IntBounds.min.z), owner.IntBounds.size));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = VariableValue.Create(new BoundsInt(new Vector3Int(owner.IntBounds.min.x, owner.IntBounds.min.y, number), owner.IntBounds.size));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(number, owner.IntBounds.size.y, owner.IntBounds.size.z)));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(owner.IntBounds.size.x, number, owner.IntBounds.size.z)));
							return SetVariableResult.Success;
						}
						case "d":
						{
							owner = VariableValue.Create(new BoundsInt(owner.IntBounds.min, new Vector3Int(owner.IntBounds.size.x, owner.IntBounds.size.y, number)));
							return SetVariableResult.Success;
						}
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
			if (right.TryGetIntBounds(out var bounds))
				return left.IntBounds == bounds;
			else
				return null;
		}
	}
}
