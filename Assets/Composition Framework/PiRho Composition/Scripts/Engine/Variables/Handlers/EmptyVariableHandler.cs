using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class EmptyVariableHandler : VariableHandler
	{
		public const string EmptyText = "(empty)";

		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(EmptyText);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			return Variable.Empty;
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			return right.IsEmpty || right.IsNullObject;
		}
	}
}