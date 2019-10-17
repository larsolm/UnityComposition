using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class ColorVariableHandler : VariableHandler
	{
		public const char Symbol = '#';

		protected internal override string ToString_(Variable variable)
		{
			var color = variable.AsColor;

			var r = Math.Round(color.r * 255);
			var g = Math.Round(color.g * 255);
			var b = Math.Round(color.b * 255);
			var a = Math.Round(color.a * 255);

			return $"{Symbol}{r:X2}{g:X2}{b:X2}{a:X2}";
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			var color = variable.AsColor;

			writer.Writer.Write(color.r);
			writer.Writer.Write(color.g);
			writer.Writer.Write(color.b);
			writer.Writer.Write(color.a);
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var r = reader.Reader.ReadSingle();
			var g = reader.Reader.ReadSingle();
			var b = reader.Reader.ReadSingle();
			var a = reader.Reader.ReadSingle();

			return Variable.Color(new Color(r, g, b, a));
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var color = owner.AsColor;

				switch (s)
				{
					case "r": return Variable.Float(color.r);
					case "g": return Variable.Float(color.g);
					case "b": return Variable.Float(color.b);
					case "a": return Variable.Float(color.a);
				}
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup.TryGetString(out var s))
				{
					var color = owner.AsColor;

					switch (s)
					{
						case "r":
						{
							owner = Variable.Color(new Color(number, color.g, color.b, color.a));
							return SetVariableResult.Success;
						}
						case "g":
						{
							owner = Variable.Color(new Color(color.r, number, color.b, color.a));
							return SetVariableResult.Success;
						}
						case "b":
						{
							owner = Variable.Color(new Color(color.r, color.g, number, color.a));
							return SetVariableResult.Success;
						}
						case "a":
						{
							owner = Variable.Color(new Color(color.r, color.g, color.b, number));
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
			if (right.TryGetColor(out var color))
				return left.AsColor == color;
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetColor(out var t))
			{
				var f = from.AsColor;
				return Vector3.Distance(new Vector3(f.r, f.g, f.b), new Vector3(t.r, t.g, t.b));
			}
			else
			{
				return 0.0f;
			}
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetColor(out var t))
				return Variable.Color(Color.Lerp(from.AsColor, t, time));
			else
				return Variable.Empty;
		}
	}
}
