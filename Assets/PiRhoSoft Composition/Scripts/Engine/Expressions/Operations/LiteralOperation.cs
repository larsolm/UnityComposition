using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LiteralOperation : Operation
	{
		private VariableValue _value;

		public LiteralOperation(VariableValue value)
		{
			_value = value;
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return _value;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(_value);
		}
	}
}
