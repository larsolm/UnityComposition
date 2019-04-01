using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LiteralOperation : Operation
	{
		public VariableValue Value;

		public LiteralOperation(VariableValue value)
		{
			Value = value;
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return Value;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Value);
		}
	}
}
