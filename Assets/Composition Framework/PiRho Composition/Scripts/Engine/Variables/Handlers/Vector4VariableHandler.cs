using PiRhoSoft.Utilities;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector4VariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable value, StringBuilder builder)
		{
			builder.Append(value.AsVector4);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var vector = variable.AsVector4;

			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
			writer.Write(vector.w);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			var w = reader.ReadSingle();

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
	}
}