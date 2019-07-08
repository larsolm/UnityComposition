using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class Vector4VariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector4());
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Vector4);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Vector4.x);
			writer.Write(value.Vector4.y);
			writer.Write(value.Vector4.z);
			writer.Write(value.Vector4.w);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();

			return VariableValue.Create(new Vector4(x, y, z, w));
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector4(out var v4))
				return VariableValue.Create(left.Vector4 + v4);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector4(out var v4))
				return VariableValue.Create(left.Vector4 - v4);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Vector4 * f);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Vector4 / number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(new Vector4(-value.Vector4.x, -value.Vector4.y, -value.Vector4.z, value.Vector4.w));
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Vector4.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Vector4.y);
				else if (lookup.String == "z") return VariableValue.Create(owner.Vector4.z);
				else if (lookup.String == "w") return VariableValue.Create(owner.Vector4.w);
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
						owner = VariableValue.Create(new Vector4(f, owner.Vector4.y, owner.Vector4.z, owner.Vector4.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Vector4(owner.Vector4.x, f, owner.Vector4.z, owner.Vector4.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "z")
					{
						owner = VariableValue.Create(new Vector4(owner.Vector4.x, owner.Vector4.y, f, owner.Vector4.w));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "w")
					{
						owner = VariableValue.Create(new Vector4(owner.Vector4.x, owner.Vector4.y, owner.Vector4.z, f));
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
			if (right.TryGetVector4(out var v4))
				return left.Vector4 == v4;
			else
				return null;
		}
	}
}
