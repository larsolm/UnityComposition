using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class BoundsVariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Bounds.center.x);
			writer.Write(value.Bounds.center.y);
			writer.Write(value.Bounds.center.z);
			writer.Write(value.Bounds.size.x);
			writer.Write(value.Bounds.size.y);
			writer.Write(value.Bounds.size.z);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();
			var d = reader.ReadSingle();

			value = VariableValue.Create(new Bounds(new Vector3(x, y, z), new Vector3(w, h, d)));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			switch (lookup)
			{
				case "x": return VariableValue.Create(owner.Bounds.center.x);
				case "y": return VariableValue.Create(owner.Bounds.center.y);
				case "z": return VariableValue.Create(owner.Bounds.center.z);
				case "w": return VariableValue.Create(owner.Bounds.size.x);
				case "h": return VariableValue.Create(owner.Bounds.size.y);
				case "d": return VariableValue.Create(owner.Bounds.size.z);
				default: return VariableValue.Empty;
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				switch (lookup)
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
	}
}
