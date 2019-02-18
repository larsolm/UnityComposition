using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class PrefixOperation : Operation
	{
		public string Symbol;
		public Operation Right;

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Symbol);
			Right.ToString(builder);
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			Right.GetInputs(inputs, source);
		}
	}
}
