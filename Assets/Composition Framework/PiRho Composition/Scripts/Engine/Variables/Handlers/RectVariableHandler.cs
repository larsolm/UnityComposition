using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RectVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsRect.ToString();
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			var rect = value.AsRect;

			writer.Write(rect.x);
			writer.Write(rect.y);
			writer.Write(rect.width);
			writer.Write(rect.height);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();

			return Variable.Rect(new Rect(x, y, w, h));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var rect = owner.AsRect;

				switch (s)
				{
					case "x": return Variable.Float(rect.x);
					case "y": return Variable.Float(rect.y);
					case "w": return Variable.Float(rect.width);
					case "h": return Variable.Float(rect.height);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetFloat(out var f))
			{
				if (lookup.TryGetString(out var s))
				{
					var rect = owner.AsRect;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Rect(new Rect(f, rect.y, rect.width, rect.height));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Rect(new Rect(rect.x, f, rect.width, rect.height));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = Variable.Rect(new Rect(rect.x, rect.y, f, rect.height));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = Variable.Rect(new Rect(rect.x, rect.y, rect.width, f));
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
			if (right.TryGetRect(out var bounds))
				return left.AsRect == bounds;
			else
				return null;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetRect(out var t))
			{
				var f = from.AsRect;
				var min = Vector2.Lerp(f.min, t.min, time);
				var size = Vector2.Lerp(f.size, t.size, time);

				return Variable.Rect(new Rect(min, size));
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}