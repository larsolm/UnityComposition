using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class EmptyVariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return SetVariableResult.NotFound;
		}
	}
}
