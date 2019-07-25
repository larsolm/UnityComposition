using PiRhoSoft.Utilities;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class RectIntVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable value, StringBuilder builder)
		{
			builder.Append(value.AsRectInt);
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			var rect = value.AsRectInt;

			writer.Write(rect.x);
			writer.Write(rect.y);
			writer.Write(rect.width);
			writer.Write(rect.height);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();

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
	}
}