using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class QuaternionVariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Quaternion.x);
			writer.Write(value.Quaternion.y);
			writer.Write(value.Quaternion.z);
			writer.Write(value.Quaternion.w);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();

			value = VariableValue.Create(new Quaternion(x, y, z, w));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			var euler = owner.Quaternion.eulerAngles;

			if (lookup == "x") return VariableValue.Create(euler.x);
			else if (lookup == "y") return VariableValue.Create(euler.y);
			else if (lookup == "z") return VariableValue.Create(euler.z);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Quaternion(number, owner.Quaternion.y, owner.Quaternion.z, owner.Quaternion.w));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Quaternion(owner.Quaternion.x, number, owner.Quaternion.z, owner.Quaternion.w));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Quaternion(owner.Quaternion.x, owner.Quaternion.y, number, owner.Quaternion.w));
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
