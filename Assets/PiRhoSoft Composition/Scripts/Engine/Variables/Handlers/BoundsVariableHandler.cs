using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class BoundsVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(new Bounds());
		protected override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.Bounds);

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Bounds.center.x);
			writer.Write(value.Bounds.center.y);
			writer.Write(value.Bounds.center.z);
			writer.Write(value.Bounds.size.x);
			writer.Write(value.Bounds.size.y);
			writer.Write(value.Bounds.size.z);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();
			var d = reader.ReadSingle();

			return VariableValue.Create(new Bounds(new Vector3(x, y, z), new Vector3(w, h, d)));
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				switch (lookup.String)
				{
					case "x": return VariableValue.Create(owner.Bounds.center.x);
					case "y": return VariableValue.Create(owner.Bounds.center.y);
					case "z": return VariableValue.Create(owner.Bounds.center.z);
					case "w": return VariableValue.Create(owner.Bounds.size.x);
					case "h": return VariableValue.Create(owner.Bounds.size.y);
					case "d": return VariableValue.Create(owner.Bounds.size.z);
				}
			}

			return VariableValue.Empty;
		}

		protected override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup.Type == VariableType.String)
				{
					switch (lookup.String)
					{
						case "x":
						{
							owner = VariableValue.Create(new Bounds(new Vector3(number, owner.Bounds.center.y, owner.Bounds.center.z), owner.Bounds.size));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = VariableValue.Create(new Bounds(new Vector3(owner.Bounds.center.x, number, owner.Bounds.center.z), owner.Bounds.size));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = VariableValue.Create(new Bounds(new Vector3(owner.Bounds.center.x, owner.Bounds.center.y, number), owner.Bounds.size));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = VariableValue.Create(new Bounds(owner.Bounds.center, new Vector3(number, owner.Bounds.size.y, owner.Bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = VariableValue.Create(new Bounds(owner.Bounds.center, new Vector3(owner.Bounds.size.x, number, owner.Bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "d":
						{
							owner = VariableValue.Create(new Bounds(owner.Bounds.center, new Vector3(owner.Bounds.size.x, owner.Bounds.size.y, number)));
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

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetBounds(out var bounds))
				return left.Bounds == bounds;
			else
				return null;
		}
	}
}
