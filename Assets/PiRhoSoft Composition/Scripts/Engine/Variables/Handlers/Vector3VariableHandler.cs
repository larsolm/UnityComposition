using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector3VariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new Vector3());
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Vector3);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Vector3.x);
			writer.Write(value.Vector3.y);
			writer.Write(value.Vector3.z);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();

			return VariableValue.Create(new Vector3(x, y, z));
		}

		protected override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector3(out var v3))
				return VariableValue.Create(left.Vector3 + v3);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector3(out var v3))
				return VariableValue.Create(left.Vector3 - v3);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Vector3 * f);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Vector3 / number);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(new Vector3(-value.Vector3.x, -value.Vector3.y, -value.Vector3.z));
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "x") return VariableValue.Create(owner.Vector3.x);
				else if (lookup.String == "y") return VariableValue.Create(owner.Vector3.y);
				else if (lookup.String == "z") return VariableValue.Create(owner.Vector3.z);
			}

			return VariableValue.Empty;
		}

		protected override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var f))
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "x")
					{
						owner = VariableValue.Create(new Vector3(f, owner.Vector3.y, owner.Vector3.z));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "y")
					{
						owner = VariableValue.Create(new Vector3(owner.Vector3.x, f, owner.Vector3.z));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "z")
					{
						owner = VariableValue.Create(new Vector3(owner.Vector3.x, owner.Vector3.y, f));
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

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetVector3(out var v3))
				return left.Vector3 == v3;
			else
				return null;
		}
	}
}
