using PiRhoSoft.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class IntVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable variable)
		{
			return variable.AsInt.ToString();
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			writer.Write(variable.AsInt);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var i = reader.ReadInt32();
			return Variable.Int(i);
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return Variable.Int(left.AsInt + i);
			else if (right.TryGetFloat(out var f))
				return Variable.Float(left.AsInt + f);
			else if (right.Type == VariableType.String)
				return Variable.String(left.AsInt + right.AsString);
			else
				return Variable.Empty;
		}

		protected internal override Variable Subtract_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return Variable.Int(left.AsInt - i);
			else if (right.TryGetFloat(out var f))
				return Variable.Float(left.AsInt - f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Multiply_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return Variable.Int(left.AsInt * i);
			else if (right.TryGetFloat(out var f))
				return Variable.Float(left.AsInt * f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Divide_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return Variable.Int(left.AsInt / i);
			else if (right.TryGetFloat(out var f))
				return Variable.Float(left.AsInt / f);
			else
				return Variable.Empty;
		}

		protected internal override Variable Modulo_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return Variable.Int(left.AsInt % i);
			else
				return Variable.Empty;
		}

		protected internal override Variable Exponent_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return Variable.Int((int)Math.Pow(left.AsInt, i));
			else if (right.TryGetFloat(out var f))
				return Variable.Float(Mathf.Pow(left.AsInt, f));
			else
				return Variable.Empty;
		}

		protected internal override Variable Negate_(Variable value)
		{
			return Variable.Int(-value.AsInt);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return left.AsInt == i;
			else
				return null;
		}

		protected internal override int? Compare_(Variable left, Variable right)
		{
			if (right.TryGetInt(out var i))
				return left.AsInt.CompareTo(i);
			else
				return null;
		}

		protected internal override float Distance_(Variable from, Variable to)
		{
			if (to.TryGetInt(out var t))
				return from.AsInt - t;
			else
				return 0.0f;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetInt(out var t))
			{
				var value = Mathf.Lerp(from.AsInt, t, time);
				return Variable.Int((int)value);
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}