using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StringVariableHandler : VariableHandler
	{
		public const char Symbol = '\"';

		protected override VariableConstraint CreateConstraint() => new StringVariableConstraint();

		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is StringVariableConstraint stringConstraint)
				return VariableValue.Create(stringConstraint.Values.Length > 0 ? stringConstraint.Values[0] : string.Empty);
			else
				return VariableValue.Create(string.Empty);
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.String);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.String);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var s = reader.ReadString();
			return VariableValue.Create(s);
		}

		protected override VariableValue Add_(VariableValue left, VariableValue right)
		{
			switch (right.Type)
			{
				case VariableType.Int: return VariableValue.Create(left.String + right.Int);
				case VariableType.Float: return VariableValue.Create(left.String + right.Float);
				case VariableType.String: return VariableValue.Create(left.String + right.String);
				default: return VariableValue.Empty;
			}
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetString(out var str))
				return left.String == str;
			else
				return null;
		}

		protected override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.String)
				return left.String.CompareTo(right.String);
			else
				return null;
		}
	}
}
