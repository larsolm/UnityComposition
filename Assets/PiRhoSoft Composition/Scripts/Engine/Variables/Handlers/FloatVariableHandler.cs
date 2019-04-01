using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableHandler : VariableHandler
	{
		protected override VariableConstraint CreateConstraint() => new FloatVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			if (constraint is FloatVariableConstraint floatConstraint)
				return VariableValue.Create(floatConstraint.Minimum);
			else
				return VariableValue.Create(0.0f);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Float);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var f = reader.ReadSingle();
			value = VariableValue.Create(f);
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
			if (right.TryGetFloat(out var number))
				return left.Float == number;
			else
				return null;
		}

		public override int? Compare(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return left.Float.CompareTo(right.Number);
			else
				return null;
		}
	}
}
