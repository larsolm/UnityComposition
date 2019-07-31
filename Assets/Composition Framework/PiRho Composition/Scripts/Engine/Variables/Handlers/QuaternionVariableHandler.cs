using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class QuaternionVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsQuaternion.ToString();
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var quaternion = variable.AsQuaternion;

			writer.Write(quaternion.x);
			writer.Write(quaternion.y);
			writer.Write(quaternion.z);
			writer.Write(quaternion.w);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();

			return Variable.Quaternion(new Quaternion(x, y, z, w));
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetQuaternion(out var q))
				return Variable.Quaternion(left.AsQuaternion * q);
			else
				return Variable.Empty;
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var quaternion = owner.AsQuaternion;

				switch (s)
				{
					case "x": return Variable.Float(quaternion.x);
					case "y": return Variable.Float(quaternion.y);
					case "z": return Variable.Float(quaternion.z);
					case "w": return Variable.Float(quaternion.w);
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
					var quaternion = owner.AsQuaternion;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Quaternion(new Quaternion(f, quaternion.y, quaternion.z, quaternion.w));
							return SetVariableResult.Success;
						}
							case "y":
							{
							owner = Variable.Quaternion(new Quaternion(quaternion.x, f, quaternion.z, quaternion.w));
							return SetVariableResult.Success;
						}
							case "z":
							{
							owner = Variable.Quaternion(new Quaternion(quaternion.x, quaternion.y, f, quaternion.w));
							return SetVariableResult.Success;
						}
							case "w":
							{
							owner = Variable.Quaternion(new Quaternion(quaternion.x, quaternion.y, quaternion.z, f));
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
			if (right.TryGetQuaternion(out var q))
				return left.AsQuaternion == q;
			else
				return null;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetQuaternion(out var t))
			{
				var value = Quaternion.Slerp(from.AsQuaternion, t, time);
				return Variable.Quaternion(value);
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}