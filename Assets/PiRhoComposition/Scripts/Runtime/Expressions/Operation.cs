using System;
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
		public abstract Variable Evaluate(IVariableMap variables);
		public abstract void ToString(StringBuilder builder);

		public virtual void GetInputs(VariableDefinitionList inputs, string source) { }
		public virtual void GetOutputs(VariableDefinitionList outputs, string source) { }

		public override string ToString()
		{
			var builder = new StringBuilder();
			ToString(builder);
			return builder.ToString();
		}
	}
}
