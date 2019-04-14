using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ColorVariableHandler : VariableHandler
	{
		public const char Symbol = '#';

		protected override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(Color.black);

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(Symbol);

			var r = Math.Round(value.Color.r * 255);
			var g = Math.Round(value.Color.g * 255);
			var b = Math.Round(value.Color.b * 255);
			var a = Math.Round(value.Color.a * 255);

			builder.AppendFormat("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Color.r);
			writer.Write(value.Color.g);
			writer.Write(value.Color.b);
			writer.Write(value.Color.a);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var r = reader.ReadSingle();
			var g = reader.ReadSingle();
			var b = reader.ReadSingle();
			var a = reader.ReadSingle();

			return VariableValue.Create(new Color(r, g, b, a));
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == "r") return VariableValue.Create(owner.Color.r);
				else if (lookup.String == "g") return VariableValue.Create(owner.Color.g);
				else if (lookup.String == "b") return VariableValue.Create(owner.Color.b);
				else if (lookup.String == "a") return VariableValue.Create(owner.Color.a);
			}

			return VariableValue.Empty;
		}

		protected override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup.Type == VariableType.String)
				{
					if (lookup.String == "r")
					{
						owner = VariableValue.Create(new Color(number, owner.Color.g, owner.Color.b, owner.Color.a));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "g")
					{
						owner = VariableValue.Create(new Color(owner.Color.r, number, owner.Color.b, owner.Color.a));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "b")
					{
						owner = VariableValue.Create(new Color(owner.Color.r, owner.Color.g, number, owner.Color.a));
						return SetVariableResult.Success;
					}
					else if (lookup.String == "a")
					{
						owner = VariableValue.Create(new Color(owner.Color.r, owner.Color.g, owner.Color.b, number));
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
			if (right.TryGetColor(out var color))
				return left.Color == color;
			else
				return null;
		}
	}
}
