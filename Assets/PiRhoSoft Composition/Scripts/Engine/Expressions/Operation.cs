using System;
using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionEvaluationException : Exception
	{
		public ExpressionEvaluationException(string error) : base(error) { }
		public ExpressionEvaluationException(string errorFormat, params object[] arguments) : this(string.Format(errorFormat, arguments)) { }
	}

	public abstract class Operation
	{
		public const string MismatchedMathType1Exception = "the operator '{0}' was passed a value of type {0} but can only operate on values of type Integer and Number";
		public const string MismatchedMathType2Exception = "the operator '{0}' was passed values of type {0} and {1} but can only operate on values of type Integer and Number";
		public const string MismatchedBooleanType1Exception = "the operator '{0}' was passed a value of type {0} but can only operate on values of type Boolean";
		public const string MismatchedBooleanType2Exception = "the operator '{0}' was passed values of type {0} and {1} but can only operate on values of type Boolean";

		public abstract VariableValue Evaluate(IVariableStore variables);
		public abstract void ToString(StringBuilder builder);
		public virtual void GetInputs(List<VariableDefinition> inputs, string source) { }
		public virtual void GetOutputs(List<VariableDefinition> outputs, string source) { }

		public override string ToString()
		{
			var builder = new StringBuilder();
			ToString(builder);
			return builder.ToString();
		}
	}
}
