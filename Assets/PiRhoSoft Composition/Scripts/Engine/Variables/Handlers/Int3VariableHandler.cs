using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Int3VariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int3.x);
			writer.Write(value.Int3.y);
			writer.Write(value.Int3.z);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();

			value = VariableValue.Create(new Vector3Int(x, y, z));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Int3.x);
			else if (lookup == "y") return VariableValue.Create(owner.Int3.y);
			else if (lookup == "z") return VariableValue.Create(owner.Int3.z);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector3Int(value.Int, owner.Int3.y, owner.Int3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector3Int(owner.Int3.x, value.Int, owner.Int3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Vector3Int(owner.Int3.x, owner.Int3.y, value.Int));
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
