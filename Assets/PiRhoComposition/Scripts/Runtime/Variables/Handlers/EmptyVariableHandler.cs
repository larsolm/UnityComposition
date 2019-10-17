using PiRhoSoft.Utilities;

namespace PiRhoSoft.Composition
{
	internal class EmptyVariableHandler : VariableHandler
	{
		public const string EmptyText = "(empty)";

		protected internal override string ToString_(Variable variable)
		{
			return EmptyText;
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			return Variable.Empty;
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			return right.IsEmpty || right.IsNullObject;
		}
	}
}
