using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class BoundsVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsBounds.ToString();
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var bounds = variable.AsBounds;

			writer.Write(bounds.center.x);
			writer.Write(bounds.center.y);
			writer.Write(bounds.center.z);
			writer.Write(bounds.size.x);
			writer.Write(bounds.size.y);
			writer.Write(bounds.size.z);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();
			var d = reader.ReadSingle();

			return Variable.Bounds(new Bounds(new Vector3(x, y, z), new Vector3(w, h, d)));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var bounds = owner.AsBounds;

				switch (s)
				{
					case "x": return Variable.Float(bounds.center.x);
					case "y": return Variable.Float(bounds.center.y);
					case "z": return Variable.Float(bounds.center.z);
					case "w": return Variable.Float(bounds.size.x);
					case "h": return Variable.Float(bounds.size.y);
					case "d": return Variable.Float(bounds.size.z);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup.TryGetString(out var s))
				{
					var bounds = owner.AsBounds;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Bounds(new Bounds(new Vector3(number, bounds.center.y, bounds.center.z), bounds.size));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Bounds(new Bounds(new Vector3(bounds.center.x, number, bounds.center.z), bounds.size));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = Variable.Bounds(new Bounds(new Vector3(bounds.center.x, bounds.center.y, number), bounds.size));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = Variable.Bounds(new Bounds(bounds.center, new Vector3(number, bounds.size.y, bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = Variable.Bounds(new Bounds(bounds.center, new Vector3(bounds.size.x, number, bounds.size.z)));
							return SetVariableResult.Success;
						}
						case "d":
						{
							owner = Variable.Bounds(new Bounds(bounds.center, new Vector3(bounds.size.x, bounds.size.y, number)));
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
			if (right.TryGetBounds(out var bounds))
				return left.AsBounds == bounds;
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetBounds(out var t))
				return Vector3.Distance(from.AsBounds.size, t.size);
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetBounds(out var t))
			{
				var f = from.AsBounds;
				var center = Vector3.Lerp(f.center, t.center, time);
				var size = Vector3.Lerp(f.size, t.size, time);

				return Variable.Bounds(new Bounds(center, size));
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}