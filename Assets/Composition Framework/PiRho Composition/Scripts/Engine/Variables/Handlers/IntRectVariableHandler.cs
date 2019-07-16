using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class IntRectVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(new RectInt());
		protected internal override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.IntRect);

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.IntRect.x);
			writer.Write(value.IntRect.y);
			writer.Write(value.IntRect.width);
			writer.Write(value.IntRect.height);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();

			return VariableValue.Create(new RectInt(x, y, w, h));
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				switch (lookup.String)
				{
					case "x": return VariableValue.Create(owner.IntRect.x);
					case "y": return VariableValue.Create(owner.IntRect.y);
					case "w": return VariableValue.Create(owner.IntRect.width);
					case "h": return VariableValue.Create(owner.IntRect.height);
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
							owner = VariableValue.Create(new RectInt(number, owner.IntRect.y, owner.IntRect.width, owner.IntRect.height));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = VariableValue.Create(new RectInt(owner.IntRect.x, number, owner.IntRect.width, owner.IntRect.height));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = VariableValue.Create(new RectInt(owner.IntRect.x, owner.IntRect.y, number, owner.IntRect.height));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = VariableValue.Create(new RectInt(owner.IntRect.x, owner.IntRect.y, owner.IntRect.width, number));
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
			if (right.TryGetIntRect(out var rect))
				return left.IntRect.x == rect.x && left.IntRect.y == rect.y && left.IntRect.width == rect.width && left.IntRect.height == rect.height;
			else
				return null;
		}
	}
}
