using PiRhoSoft.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class EnumVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsEnum.ToString();
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			data.SaveEnum(writer, variable.AsEnum);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var e = data.LoadEnum(reader);
			return Variable.Enum(e);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetEnum(left.EnumType, out var e))
				return left.AsEnum == e;
			else
				return null;
		}

		protected internal override int? Compare_(Variable left, Variable right)
		{
			if (right.TryGetEnum(left.EnumType, out var e))
				return left.AsEnum.CompareTo(e);
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetEnum(from.EnumType, out var t))
			{
				var fInt = (int)Enum.Parse(from.EnumType, from.AsEnum.ToString());
				var tInt = (int)Enum.Parse(to.EnumType, t.ToString());

				return tInt = fInt;
			}
			else
			{
				return 0.0f;
			}
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetEnum(from.EnumType, out var t))
			{
				var fInt = (int)Enum.Parse(from.EnumType, from.AsEnum.ToString());
				var tInt = (int)Enum.Parse(to.EnumType, t.ToString());
				var value = Mathf.Lerp(fInt, tInt, time);

				return Variable.Int((int)value);
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}
