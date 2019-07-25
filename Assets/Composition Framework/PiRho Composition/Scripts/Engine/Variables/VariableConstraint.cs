using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public abstract class VariableConstraint : ISerializableData
	{
		public abstract VariableType Type { get; }
		public abstract Variable Generate();
		public abstract bool IsValid(Variable value);
		public abstract void Save(BinaryWriter writer, SerializedData data);
		public abstract void Load(BinaryReader reader, SerializedData data);

		public static VariableConstraint Create(VariableType type)
		{
			switch (type)
			{
				case VariableType.Enum: return new EnumConstraint();
				case VariableType.Float: return new FloatConstraint();
				case VariableType.Int: return new IntConstraint();
				case VariableType.List: return new ListConstraint();
				case VariableType.Object: return new ObjectConstraint();
				case VariableType.Other: return new OtherConstraint();
				case VariableType.Store: return new StoreConstraint();
				case VariableType.String: return new StringConstraint();
				default: return null;
			}
		}
	}
}