using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Int2VariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector2Int());
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int2.x);
			writer.Write(value.Int2.y);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();

			value = VariableValue.Create(new Vector2Int(x, y));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Int2.x);
			else if (lookup == "y") return VariableValue.Create(owner.Int2.y);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector2Int(value.Int, owner.Int2.y));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector2Int(owner.Int2.x, value.Int));
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.NotFound;
				}
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}
	}
}
