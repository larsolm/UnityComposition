using PiRhoSoft.Utilities;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class Vector2VariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsVector2);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var vector = variable.AsVector2;

			writer.Write(vector.x);
			writer.Write(vector.y);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();

			return Variable.Vector2(new Vector2(x, y));
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			if (right.TryGetVector2(out var vector))
				return Variable.Vector2(left.AsVector2 + vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Subtract_(Variable left, Variable right)
		{
			if (right.TryGetVector2(out var vector))
				return Variable.Vector2(left.AsVector2 - vector);
			else
				return Variable.Empty;
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var f))
				return Variable.Vector2(left.AsVector2 * f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Divide_(Variable left, Variable right)
		{
			if (right.TryGetFloat(out var f))
				return Variable.Vector2(left.AsVector2 / f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Negate_(Variable value)
		{
			var vector = value.AsVector2;
			return Variable.Vector2(new Vector2(-vector.x, -vector.y));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var vector = owner.AsVector2;

				switch (s)
				{
					case "x": return Variable.Float(vector.x);
					case "y": return Variable.Float(vector.y);
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
					var vector = owner.AsVector2;

					switch (s)
					{
						case "x":
						{
							owner = Variable.Vector2(new Vector2(f, vector.y));
							return SetVariableResult.Success;
						}
						case "y":
						{
							owner = Variable.Vector2(new Vector2(vector.x, f));
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
			if (right.TryGetVector2(out var vector))
				return left.AsVector2 == vector;
			else
				return null;
		}
	}
}