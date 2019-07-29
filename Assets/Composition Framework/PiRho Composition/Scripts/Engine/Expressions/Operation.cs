using System;
using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.Composition
{
	public class ExpressionEvaluationException : Exception
	{
		public ExpressionEvaluationException(string error) : base(error) { }
		public ExpressionEvaluationException(string errorFormat, params object[] arguments) : this(string.Format(errorFormat, arguments)) { }
	}

	public abstract class Operation
	{
		public abstract void Parse(ExpressionParser parser, ExpressionToken token);
		public abstract Variable Evaluate(IVariableCollection variables);
		public abstract void ToString(StringBuilder builder);

		public virtual void GetInputs(IList<VariableDefinition> inputs, string source) { }
		public virtual void GetOutputs(IList<VariableDefinition> outputs, string source) { }

		public override string ToString()
		{
			var builder = new StringBuilder();
			ToString(builder);
			return builder.ToString();
		}
	}
}
