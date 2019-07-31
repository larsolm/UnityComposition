using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class StringVariableHandler : VariableHandler
	{
		public const char Symbol = '\"';

		protected internal override string ToString_(Variable variable)
		{
			return variable.AsString;
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			writer.Write(variable.AsString);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var s = reader.ReadString();
			return Variable.String(s);
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			return Variable.String(left.AsString + right.ToString());
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetString(out var str))
				return left.AsString == str;
			else
				return null;
		}

		protected internal override int? Compare_(Variable left, Variable right)
		{
			if (right.TryGetString(out var s))
				return left.AsString.CompareTo(s);
			else
				return null;
		}

		protected internal override Variable Interpolate_(Variable from, Variable to, float time)
		{
			if (to.TryGetString(out var t))
			{
				var f = from.AsString;
				var total = Mathf.Max(f.Length, t.Length);
				var length = (int)Mathf.Lerp(0, total, time);

				if (f.Length < length) f = f.PadRight(length);
				if (t.Length < length) t = t.PadRight(length);

				var start = f.Substring(0, length);
				var end = t.Substring(length);

				return Variable.String(start + end);
			}
			else
			{
				return Variable.Empty;
			}
		}
	}
}