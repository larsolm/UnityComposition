using System;

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
	}
}
