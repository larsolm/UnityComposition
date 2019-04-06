using System;

namespace PiRhoSoft.CompositionEngine
{
	public class EnumVariableConstraint : VariableConstraint
	{
		public Type Type;

		public override string Write()
		{
			return Type != null ? Type.AssemblyQualifiedName : string.Empty;
		}

		public override bool Read(string data)
		{
			Type = System.Type.GetType(data, false);
			return Type != null;
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
