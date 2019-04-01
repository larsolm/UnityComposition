using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableHandler : VariableHandler
	{
		protected override VariableConstraint CreateConstraint() => new IntVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			if (constraint is IntVariableConstraint intConstraint)
				return VariableValue.Create(intConstraint.Minimum);
			else
				return VariableValue.Create(0);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var i = reader.ReadInt32();
			value = VariableValue.Create(i);
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
			if (right.Type == VariableType.Int)
				return left.Int == right.Int;
			else
				return null;
		}

		public override int? Compare(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.Int)
				return left.Int.CompareTo(right.Int);
			else
				return null;
		}
	}
}
