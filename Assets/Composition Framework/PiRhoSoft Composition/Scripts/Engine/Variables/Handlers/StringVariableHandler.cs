using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class StringVariableHandler : VariableHandler
	{
		public const char Symbol = '\"';

		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is StringVariableConstraint stringConstraint)
				return VariableValue.Create(stringConstraint.Values.Count > 0 ? stringConstraint.Values[0] : string.Empty);
			else
				return VariableValue.Create(string.Empty);
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.String);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.String);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var s = reader.ReadString();
			return VariableValue.Create(s);
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Int: return VariableValue.Create(left.String + right.Int);
				case VariableType.Float: return VariableValue.Create(left.String + right.Float);
				case VariableType.String: return VariableValue.Create(left.String + right.String);
				default: return VariableValue.Empty;
			}
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetString(out var str))
				return left.String == str;
			else
				return null;
		}

		protected internal override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.String)
				return left.String.CompareTo(right.String);
			else
				return null;
		}
	}
}
