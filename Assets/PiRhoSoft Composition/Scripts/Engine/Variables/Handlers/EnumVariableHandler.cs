using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableHandler : VariableHandler
	{
		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			// saving as a string is the simplest way of handling enums with non Int32 underlying type

			writer.Write(value.EnumType.AssemblyQualifiedName);
			writer.Write(value.Enum.ToString());
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var typeName = reader.ReadString();
			var enumValue = reader.ReadString();

			var type = Type.GetType(typeName);
			value = VariableValue.Create((Enum)Enum.Parse(type, enumValue));
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
