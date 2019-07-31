using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector3IntVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsVector3Int.ToString();
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			var vector = value.AsVector3Int;

			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var z = reader.ReadInt32();

			return Variable.Vector3Int(new Vector3Int(x, y, z));
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			if (right.TryGetVector3Int(out var vector))
				return Variable.Vector3Int(left.AsVector3Int + vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Subtract_(Variable left, Variable right)
		{
			if (right.TryGetVector3Int(out var vector))
				return Variable.Vector3Int(left.AsVector3Int - vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return Variable.Vector3Int(left.AsVector3Int * i);
			else
				return Variable.Empty;
		}

		protected internal override Variable Divide_(Variable left, Variable right)
		{
			var vector = left.AsVector3Int;

			if (right.TryGetInt(out var i) && i != 0)
				return Variable.Vector3Int(new Vector3Int(vector.x / i, vector.y / i, vector.z / i));
			else if (right.TryGetFloat(out var f))
				return Variable.Vector3(new Vector3(vector.x / f, vector.y / f, vector.z / f));
			else
				return Variable.Empty;
		}

		protected internal override Variable Negate_(Variable value)
		{
			var vector = value.AsVector3Int;
			return Variable.Vector3Int(new Vector3Int(-vector.x, -vector.y, -vector.z));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var vector = owner.AsVector3Int;

				switch (s)
				{
					case "x": return Variable.Int(vector.x);
					case "y": return Variable.Int(vector.y);
					case "z": return Variable.Int(vector.z);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetInt(out var i))
			{
				if (lookup.TryGetString(out var s))
				{
					var vector = owner.AsVector3Int;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Vector3Int(new Vector3Int(i, vector.y, vector.z));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Vector3Int(new Vector3Int(vector.x, i, vector.z));
							return SetVariableResult.Success;
						}
						case "z":
						{
							owner = Variable.Vector3Int(new Vector3Int(vector.x, vector.y, i));
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
			if (right.TryGetVector3Int(out var vector))
				return left.AsVector3Int == vector;
			else
				return null;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetVector3Int(out var t))
			{
				var lerped = Vector3.Lerp(from.AsVector3Int, t, time);
				return Variable.Vector3Int(new Vector3Int((int)lerped.x, (int)lerped.y, (int)lerped.z));
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}