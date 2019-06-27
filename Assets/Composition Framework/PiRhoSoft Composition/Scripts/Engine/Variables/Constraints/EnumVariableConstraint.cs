using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableConstraint : VariableConstraint
	{
		public Type Type = null;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(Type?.AssemblyQualifiedName ?? string.Empty);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var type = reader.ReadString();
			Type = Type.GetType(type);
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

		public override bool Equals(object obj)
		{
			return obj is EnumVariableConstraint other
				&& Type == other.Type;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
