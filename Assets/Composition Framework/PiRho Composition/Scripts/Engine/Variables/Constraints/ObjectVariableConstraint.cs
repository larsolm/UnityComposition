using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public class ObjectVariableConstraint : VariableConstraint
	{
		public Type Type = typeof(Object);

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Type?.AssemblyQualifiedName ?? string.Empty);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var type = reader.ReadString();
			Type = Type.GetType(type) ?? typeof(Object);
		}

		public override bool IsValid(VariableValue value)
		{
			return Type != null && (value.IsNull || Type.IsAssignableFrom(value.ReferenceType));
		}

		public override bool Equals(object obj)
		{
			return obj is ObjectVariableConstraint other
				&& Type == other.Type;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
