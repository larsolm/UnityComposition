using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class StringVariableHandler : VariableHandler
	{
		public const char Symbol = '\"';

		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsString);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			writer.Write(variable.AsString);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var s = reader.ReadString();
			return Variable.String(s);
		}

		protected internal override Variable Add_(Variable left, Variable right)
		{
			return Variable.String(left.AsString + right.ToString());
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetString(out var str))
				return left.AsString == str;
			else
				return null;
		}

		protected internal override int? Compare_(Variable left, Variable right)
		{
			if (right.TryGetString(out var s))
				return left.AsString.CompareTo(s);
			else
				return null;
		}
	}
}