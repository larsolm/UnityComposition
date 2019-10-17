using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector4VariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsVector4.ToString();
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			var vector = variable.AsVector4;

			writer.Writer.Write(vector.x);
			writer.Writer.Write(vector.y);
			writer.Writer.Write(vector.z);
			writer.Writer.Write(vector.w);
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var x = reader.Reader.ReadSingle();
			var y = reader.Reader.ReadSingle();
			var z = reader.Reader.ReadSingle();
			var w = reader.Reader.ReadSingle();

			return Variable.Vector4(new Vector4(x, y, z, w));
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			if (right.TryGetVector4(out var vector))
				return Variable.Vector4(left.AsVector4 + vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Subtract_(Variable left, Variable right)
		{
			if (right.TryGetVector4(out var vector))
				return Variable.Vector4(left.AsVector4 - vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var f))
				return Variable.Vector4(left.AsVector4 * f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Divide_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var number))
				return Variable.Vector4(left.AsVector4 / number);
			else
				return Variable.Empty;
		}

		protected internal override Variable Negate_(Variable value)
		{
			var vector = value.AsVector4;
			return Variable.Vector4(new Vector4(-vector.x, -vector.y, -vector.z, vector.w));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var vector = owner.AsVector4;

				switch (s)
				{
					case "x": return Variable.Float(vector.x);
					case "y": return Variable.Float(vector.y);
					case "z": return Variable.Float(vector.z);
					case "w": return Variable.Float(vector.w);
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
					var vector = owner.AsVector4;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Vector4(new Vector4(f, vector.y, vector.z, vector.w));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Vector4(new Vector4(vector.x, f, vector.z, vector.w));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = Variable.Vector4(new Vector4(vector.x, vector.y, f, vector.w));
							return SetVariableResult.Success;
						}
						case "w":
						{
							owner = Variable.Vector4(new Vector4(vector.x, vector.y, vector.z, f));
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
			if (right.TryGetVector4(out var vector))
				return left.AsVector4 == vector;
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetVector4(out var t))
				return Vector4.Distance(from.AsVector4, t);
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetVector4(out var t))
			{
				var lerped = Vector4.Lerp(from.AsVector4, t, time);
				return Variable.Vector4(lerped);
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}
