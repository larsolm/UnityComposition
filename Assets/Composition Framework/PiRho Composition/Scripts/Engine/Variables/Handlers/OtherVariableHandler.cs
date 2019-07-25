using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class OtherVariableHandler : VariableHandler
	{
		public const string NullText = "(null)";

		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			if (variable.IsNullOther)
				builder.Append(NullText);
			else
				builder.Append(variable.AsOther);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			data.SaveObject(writer, variable.AsOther);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var other = data.LoadObject<object>(reader);
			return Variable.Other(other);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (owner.IsNullOther)
				return Variable.Empty;
			else
				return Lookup(owner.AsOther, lookup);
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (owner.IsNullOther)
				return SetVariableResult.NotFound;
			else
				return Apply(owner.AsOther, lookup, value);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.IsEmpty || right.IsNullOther)
				return left.IsNullOther;
			else if (right.TryGetOther(out object other))
				return left.AsOther == other;
			else
				return null;
		}
	}
}