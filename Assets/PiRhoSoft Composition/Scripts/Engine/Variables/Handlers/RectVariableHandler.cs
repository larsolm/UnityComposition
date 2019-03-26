using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RectVariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(new Rect());
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.IntRect.position.x);
			writer.Write(value.IntRect.position.y);
			writer.Write(value.IntRect.size.x);
			writer.Write(value.IntRect.size.y);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var w = reader.ReadSingle();
			var h = reader.ReadSingle();

			value = VariableValue.Create(new Rect(x, y, w, h));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			switch (lookup)
			{
				case "x": return VariableValue.Create(owner.Rect.position.x);
				case "y": return VariableValue.Create(owner.Rect.position.y);
				case "w": return VariableValue.Create(owner.Rect.size.x);
				case "h": return VariableValue.Create(owner.Rect.size.y);
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
						owner = VariableValue.Create(new Rect(number, owner.Rect.position.y, owner.Rect.size.x, owner.Rect.size.y));
						return SetVariableResult.Success;
					}
					case "y":
					{
						owner = VariableValue.Create(new Rect(owner.Rect.position.x, number, owner.Rect.size.x, owner.Rect.size.y));
						return SetVariableResult.Success;
					}
					case "w":
					{
						owner = VariableValue.Create(new Rect(owner.Rect.position.x, owner.Rect.position.y, number, owner.Rect.size.y));
						return SetVariableResult.Success;
					}
					case "h":
					{
						owner = VariableValue.Create(new Rect(owner.Rect.position.x, owner.Rect.position.y, owner.Rect.size.x, number));
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
