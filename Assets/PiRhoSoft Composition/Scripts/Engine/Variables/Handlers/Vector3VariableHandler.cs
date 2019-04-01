using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector3VariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(Vector3.zero);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Vector3.x);
			writer.Write(value.Vector3.y);
			writer.Write(value.Vector3.z);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();

			value = VariableValue.Create(new Vector3(x, y, z));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Vector3.x);
			else if (lookup == "y") return VariableValue.Create(owner.Vector3.y);
			else if (lookup == "z") return VariableValue.Create(owner.Vector3.z);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector3(number, owner.Vector3.y, owner.Vector3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector3(owner.Vector3.x, number, owner.Vector3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Vector3(owner.Vector3.x, owner.Vector3.y, number));
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

		public override bool? IsEqual(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector3(out var vector3))
				return left.Vector3 == vector3;
			else
				return null;
		}
	}
}
