using System;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableConstraint : VariableConstraint
	{
		public Type Type;

		public override string Write()
		{
			return Type != null ? Type.AssemblyQualifiedName : string.Empty;
		}

		public override bool Read(string data)
		{
			Type = Type.GetType(data, false);
			return Type != null;
		}

		public override bool IsValid(VariableValue value)
		{
			return Type != null && Type.IsAssignableFrom(value.ReferenceType);
		}

		#region Editor Interface

		public List<Type> Types;

		public void SetType(int tab, int selection)
		{
			// Account for none
			Type = selection <= 0 ? null : Types[selection - 1];
		}

		#endregion
	}
}
