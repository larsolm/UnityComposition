using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RectVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(new Rect());
		protected override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.Rect);

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Rect.x);
			writer.Write(value.Rect.y);
			writer.Write(value.Rect.width);
			writer.Write(value.Rect.height);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();

			return VariableValue.Create(new Rect(x, y, w, h));
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				switch (lookup.String)
				{
					case "x": return VariableValue.Create(owner.Rect.x);
					case "y": return VariableValue.Create(owner.Rect.y);
					case "w": return VariableValue.Create(owner.Rect.width);
					case "h": return VariableValue.Create(owner.Rect.height);
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
							owner = VariableValue.Create(new Rect(number, owner.Rect.y, owner.Rect.width, owner.Rect.height));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = VariableValue.Create(new Rect(owner.Rect.x, number, owner.Rect.width, owner.Rect.height));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = VariableValue.Create(new Rect(owner.Rect.x, owner.Rect.y, number, owner.Rect.height));
							return SetVariableResult.Success;
						}
						case "h":
						{
							owner = VariableValue.Create(new Rect(owner.Rect.x, owner.Rect.y, owner.Rect.width, number));
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
			if (right.TryGetRect(out var bounds))
				return left.Rect == bounds;
			else
				return null;
		}
	}
}
