using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableConstraint : VariableConstraint
	{
		public Type Type;
		
		public override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Type != null ? Type.AssemblyQualifiedName : string.Empty);
		}

		public override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var type = reader.ReadString();
			Type = Type.GetType(type, true);
		}

		public override bool IsValid(VariableValue value)
		{
			return Type != null && (value.IsNull || Type.IsAssignableFrom(value.ReferenceType));
		}
	}
}
