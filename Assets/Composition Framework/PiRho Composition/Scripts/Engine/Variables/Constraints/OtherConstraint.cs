using PiRhoSoft.Utilities;
using System;
using System.IO;

namespace PiRhoSoft.Composition
{
	public class OtherConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Other;

		private Type _otherType;

		public Type OtherType
		{
			get => _otherType;
			set => _otherType = value ?? typeof(object);
		}

		public OtherConstraint()
		{
			OtherType = null;
		}

		public OtherConstraint(Type type)
		{
			OtherType = type;
		}

		public override Variable Generate()
		{
			return Variable.Other(null);
		}

		public override bool IsValid(Variable variable)
		{
			return variable.IsNullOther || variable.HasOther(OtherType);
		}

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			data.SaveType(writer, OtherType);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			OtherType = data.LoadType(reader);
		}
	}
}