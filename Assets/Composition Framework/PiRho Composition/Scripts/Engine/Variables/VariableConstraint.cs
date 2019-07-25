using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public abstract class VariableConstraint : ISerializableData
	{
		public abstract Variable Generate();
		public abstract bool IsValid(Variable value);
		public abstract void Save(BinaryWriter writer, SerializedData data);
		public abstract void Load(BinaryReader reader, SerializedData data);
	}
}