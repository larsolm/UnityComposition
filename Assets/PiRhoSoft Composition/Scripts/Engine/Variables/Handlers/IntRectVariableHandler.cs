using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntRectVariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(new RectInt());
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
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var w = reader.ReadInt32();
			var h = reader.ReadInt32();

			value = VariableValue.Create(new RectInt(x, y, w, h));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			switch (lookup)
			{
				case "x": return VariableValue.Create(owner.IntRect.position.x);
				case "y": return VariableValue.Create(owner.IntRect.position.y);
				case "w": return VariableValue.Create(owner.IntRect.size.x);
				case "h": return VariableValue.Create(owner.IntRect.size.y);
				default: return VariableValue.Empty;
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				switch (lookup)
				{
					case "x":
					{
						owner = VariableValue.Create(new RectInt(value.Int, owner.IntRect.position.y, owner.IntRect.size.x, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "y":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, value.Int, owner.IntRect.size.x, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "w":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, owner.IntRect.position.y, value.Int, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "h":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, owner.IntRect.position.y, owner.IntRect.size.x, value.Int));
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
