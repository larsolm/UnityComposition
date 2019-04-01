using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StringVariableHandler : VariableHandler
	{
		protected override VariableConstraint CreateConstraint() => new StringVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			if (constraint is StringVariableConstraint stringConstraint)
				return VariableValue.Create(stringConstraint.Values.Length > 0 ? stringConstraint.Values[0] : string.Empty);
			else
				return VariableValue.Create(string.Empty);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.String);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var s = reader.ReadString();
			value = VariableValue.Create(s);
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return SetVariableResult.NotFound;
		}

		public override bool? IsEqual(VariableValue left, VariableValue right)
		{
			if (right.TryGetString(out var str))
				return left.String == str;
			else
				return null;
		}

		public override int? Compare(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.String)
				return left.String.CompareTo(right.String);
			else
				return null;
		}
	}
}
