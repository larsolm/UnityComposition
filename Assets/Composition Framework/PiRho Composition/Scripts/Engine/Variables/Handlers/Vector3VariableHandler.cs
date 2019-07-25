using PiRhoSoft.Utilities;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector3VariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsVector3);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var vector = variable.AsVector3;

			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();

			return Variable.Vector3(new Vector3(x, y, z));
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			if (right.TryGetVector3(out var vector))
				return Variable.Vector3(left.AsVector3 + vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Subtract_(Variable left, Variable right)
		{
			if (right.TryGetVector3(out var vector))
				return Variable.Vector3(left.AsVector3 - vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var f))
				return Variable.Vector3(left.AsVector3 * f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Divide_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var number))
				return Variable.Vector3(left.AsVector3 / number);
			else
				return Variable.Empty;
		}

		protected internal override Variable Negate_(Variable value)
		{
			var vector = value.AsVector3;

			return Variable.Vector3(new Vector3(-vector.x, -vector.y, -vector.z));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var vector = owner.AsVector3;

				switch (s)
				{
					case "x": return Variable.Float(vector.x);
					case "y": return Variable.Float(vector.y);
					case "z": return Variable.Float(vector.z);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetFloat(out var f))
			{
				if (lookup.TryGetString(out var s))
				{
					var vector = owner.AsVector3;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Vector3(new Vector3(f, vector.y, vector.z));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Vector3(new Vector3(vector.x, f, vector.z));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = Variable.Vector3(new Vector3(vector.x, vector.y, f));
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

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetVector3(out var vector))
				return left.AsVector3 == vector;
			else
				return null;
		}
	}
}