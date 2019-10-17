using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RectIntVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsRectInt.ToString();
		}

		protected internal override void Save_(Variable value, SerializedDataWriter writer)
		{
			var rect = value.AsRectInt;

			writer.Writer.Write(rect.x);
			writer.Writer.Write(rect.y);
			writer.Writer.Write(rect.width);
			writer.Writer.Write(rect.height);
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var x = reader.Reader.ReadInt32();
			var y = reader.Reader.ReadInt32();
			var w = reader.Reader.ReadInt32();
			var h = reader.Reader.ReadInt32();

			return Variable.RectInt(new RectInt(x, y, w, h));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var rect = owner.AsRectInt;

				switch (s)
				{
					case "x": return Variable.Int(rect.x);
					case "y": return Variable.Int(rect.y);
					case "w": return Variable.Int(rect.width);
					case "h": return Variable.Int(rect.height);
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
					var rect = owner.AsRectInt;

					switch (s)
					{
						case "x":
						{
							owner = Variable.RectInt(new RectInt(number, rect.y, rect.width, rect.height));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.RectInt(new RectInt(rect.x, number, rect.width, rect.height));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = Variable.RectInt(new RectInt(rect.x, rect.y, number, rect.height));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = Variable.RectInt(new RectInt(rect.x, rect.y, rect.width, number));
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
			var leftRect = left.AsRectInt;

			if (right.TryGetRectInt(out var rightRect))
				return leftRect.x == rightRect.x && leftRect.y == rightRect.y && leftRect.width == rightRect.width && leftRect.height == rightRect.height;
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetRectInt(out var t))
				return Vector2.Distance(from.AsRectInt.size, t.size);
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetRectInt(out var t))
			{
				var f = from.AsRectInt;
				var min = Vector2.Lerp(f.min, t.min, time);
				var size = Vector2.Lerp(f.size, t.size, time);

				return Variable.RectInt(new RectInt((int)min.x, (int)min.y, (int)size.x, (int)size.y));
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}
