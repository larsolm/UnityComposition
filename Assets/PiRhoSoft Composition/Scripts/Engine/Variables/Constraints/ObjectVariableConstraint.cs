using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableConstraint : VariableConstraint
	{
		public Type Type;

		public override string Write(IList<Object> objects)
		{
			return Type != null ? Type.AssemblyQualifiedName : string.Empty;
		}

		public override bool Read(string data, IList<Object> objects)
		{
			Type = Type.GetType(data, false);
			return Type != null;
		}

		public override bool IsValid(VariableValue value)
		{
			return Type != null && (value.IsNull || Type.IsAssignableFrom(value.ReferenceType));
		}
	}
}
