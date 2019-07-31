using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class BoundsIntVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsBoundsInt.ToString();
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var bounds = variable.AsBoundsInt;

			writer.Write(bounds.min.x);
			writer.Write(bounds.min.y);
			writer.Write(bounds.min.z);
			writer.Write(bounds.size.x);
			writer.Write(bounds.size.y);
			writer.Write(bounds.size.z);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();
			var d = reader.ReadInt32();

			return Variable.BoundsInt(new BoundsInt(new Vector3Int(x, y, z), new Vector3Int(w, h, d)));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var bounds = owner.AsBoundsInt;

				switch (s)
				{
					case "x": return Variable.Int(bounds.min.x);
					case "y": return Variable.Int(bounds.min.y);
					case "z": return Variable.Int(bounds.min.z);
					case "w": return Variable.Int(bounds.size.x);
					case "h": return Variable.Int(bounds.size.y);
					case "d": return Variable.Int(bounds.size.z);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetInt(out var number))
			{
				if (lookup.TryGetString(out var s))
				{
					var bounds = owner.AsBoundsInt;

					switch (s)
					{
						case "x":
						{
							owner = Variable.BoundsInt(new BoundsInt(new Vector3Int(number, bounds.min.y, bounds.min.z), bounds.size));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.BoundsInt(new BoundsInt(new Vector3Int(bounds.min.x, number, bounds.min.z), bounds.size));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = Variable.BoundsInt(new BoundsInt(new Vector3Int(bounds.min.x, bounds.min.y, number), bounds.size));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = Variable.BoundsInt(new BoundsInt(bounds.min, new Vector3Int(number, bounds.size.y, bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = Variable.BoundsInt(new BoundsInt(bounds.min, new Vector3Int(bounds.size.x, number, bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "d":
						{
							owner = Variable.BoundsInt(new BoundsInt(bounds.min, new Vector3Int(bounds.size.x, bounds.size.y, number)));
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

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetBoundsInt(out var bounds))
				return left.AsBoundsInt == bounds;
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetBoundsInt(out var t))
				return Vector3.Distance(from.AsBoundsInt.size, t.size);
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetBoundsInt(out var t))
			{
				var f = from.AsBoundsInt;
				var min = Vector3.Lerp(f.min, t.min, time);
				var size = Vector3.Lerp(f.size, t.size, time);

				return Variable.BoundsInt(new BoundsInt((int)min.x, (int)min.y, (int)min.z, (int)size.x, (int)size.y, (int)size.z));
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}