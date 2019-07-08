using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class QuaternionVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(Quaternion.identity);
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Quaternion);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Quaternion.x);
			writer.Write(value.Quaternion.y);
			writer.Write(value.Quaternion.z);
			writer.Write(value.Quaternion.w);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();

			return VariableValue.Create(new Quaternion(x, y, z, w));
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetQuaternion(out var q))
				return VariableValue.Create(left.Quaternion * q);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Quaternion.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Quaternion.y);
				else if (lookup.String == "z") return VariableValue.Create(owner.Quaternion.z);
				else if (lookup.String == "w") return VariableValue.Create(owner.Quaternion.w);
			}

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var f))
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "x")
					{
						owner = VariableValue.Create(new Quaternion(f, owner.Quaternion.y, owner.Quaternion.z, owner.Quaternion.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Quaternion(owner.Quaternion.x, f, owner.Quaternion.z, owner.Quaternion.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "z")
					{
						owner = VariableValue.Create(new Quaternion(owner.Quaternion.x, owner.Quaternion.y, f, owner.Quaternion.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "w")
					{
						owner = VariableValue.Create(new Quaternion(owner.Quaternion.x, owner.Quaternion.y, owner.Quaternion.z, f));
						return SetVariableResult.Success;
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
			if (right.TryGetQuaternion(out var q))
				return left.Quaternion == q;
			else
				return null;
		}
	}
}
