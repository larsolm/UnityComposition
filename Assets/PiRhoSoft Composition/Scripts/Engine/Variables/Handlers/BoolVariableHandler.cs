using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class BoolVariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(false);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Bool);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var b = reader.ReadBoolean();
			value = VariableValue.Create(b);
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
			if (right.Type == VariableType.Bool)
				return left.Bool == right.Bool;
			else
				return null;
		}
	}
}
