using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableConstraint : VariableConstraint
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
			if (Type != null)
			{
				if (value.EnumType == Type)
				{
					return true;
				}
				else if (value.Type == VariableType.String)
				{
					try
					{
						var _ = (Enum)Enum.Parse(Type, value.String);
						return true;
					}
					catch
					{
					}
				}
			}

			return false;
		}
	}
}
