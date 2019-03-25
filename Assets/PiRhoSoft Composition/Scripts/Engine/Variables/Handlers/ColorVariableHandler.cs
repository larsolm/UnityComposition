using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ColorVariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Color.r);
			writer.Write(value.Color.g);
			writer.Write(value.Color.b);
			writer.Write(value.Color.a);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var r = reader.ReadSingle();
			var g = reader.ReadSingle();
			var b = reader.ReadSingle();
			var a = reader.ReadSingle();

			value = VariableValue.Create(new Color(r, g, b, a));
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "r") return VariableValue.Create(owner.Color.r);
			else if (lookup == "g") return VariableValue.Create(owner.Color.g);
			else if (lookup == "b") return VariableValue.Create(owner.Color.b);
			else if (lookup == "a") return VariableValue.Create(owner.Color.a);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup == "r")
				{
					owner = VariableValue.Create(new Color(number, owner.Color.g, owner.Color.b, owner.Color.a));
					return SetVariableResult.Success;
				}
				else if (lookup == "g")
				{
					owner = VariableValue.Create(new Color(owner.Color.r, number, owner.Color.b, owner.Color.a));
					return SetVariableResult.Success;
				}
				else if (lookup == "b")
				{
					owner = VariableValue.Create(new Color(owner.Color.r, owner.Color.g, number, owner.Color.a));
					return SetVariableResult.Success;
				}
				else if (lookup == "a")
				{
					owner = VariableValue.Create(new Color(owner.Color.r, owner.Color.g, owner.Color.b, number));
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.NotFound;
				}
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}
	}
}
