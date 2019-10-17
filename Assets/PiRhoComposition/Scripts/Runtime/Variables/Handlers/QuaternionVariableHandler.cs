using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class QuaternionVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsQuaternion.ToString();
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			var quaternion = variable.AsQuaternion;

			writer.Writer.Write(quaternion.x);
			writer.Writer.Write(quaternion.y);
			writer.Writer.Write(quaternion.z);
			writer.Writer.Write(quaternion.w);
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var x = reader.Reader.ReadSingle();
			var y = reader.Reader.ReadSingle();
			var z = reader.Reader.ReadSingle();
			var w = reader.Reader.ReadSingle();

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

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetQuaternion(out var t))
				return Quaternion.Angle(from.AsQuaternion, t);
			else
				return 0.0f;
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
