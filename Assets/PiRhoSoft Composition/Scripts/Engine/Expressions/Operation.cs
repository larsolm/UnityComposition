using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionEvaluationException : Exception
	{
		public ExpressionEvaluationException(string error) : base(error) { }
		public ExpressionEvaluationException(string errorFormat, params object[] arguments) : this(string.Format(errorFormat, arguments)) { }

		#region Errors

		private const string _invalidAssignmentException = "values can only be assigned to variables";
		private const string _missingAssignmentException = "the variable '{0}' could not be found";
		private const string _readOnlyAssignmentException = "the variable '{0}' is read only and cannot be assigned";
		private const string _mismatchedAssignmentException = "the variable '{0}' cannot be assigned a value of type {1}";
		private const string _prefixTypeMismatch1Exception = "the operator '{0}' was passed a value of type {1} but can only operate on values of type {2}";
		private const string _prefixTypeMismatch2Exception = "the operator '{0}' was passed a value of type {1} but can only operate on values of type {2} and {3}";
		private const string _prefixTypeMismatchXException = "the operator '{0}' was passed a value of type {1} but can only operate on values of type {2}, and {3}";
		private const string _infixTypeMismatch1Exception = "the operator '{0}' was passed values of type {1} and {2} but can only operate on values of type {3}";
		private const string _infixTypeMismatch2Exception = "the operator '{0}' was passed values of type {1} and {2} but can only operate on values of type {3} and {4}";
		private const string _infixTypeMismatchXException = "the operator '{0}' was passed values of type {1} and {2} but can only operate on values of type {3}, and {4}";
		private const string _comparisonTypeMismatch1Exception = "the operator '{0}' was passed values of type {1} and {2} but type {1} can only be compared with values of type {3}";
		private const string _comparisonTypeMismatch2Exception = "the operator '{0}' was passed values of type {1} and {2} but type {1} can only be compared with values of type {3} and {4}";
		private const string _comparisonTypeMismatchXException = "the operator '{0}' was passed values of type {1} and {2} but type {1} can only be compared with values of type {3}, and {4}";
		private const string _divideByZeroException = "the operator '{0}' attempted to divide by 0";

		public static ExpressionEvaluationException InvalidAssignment(string symbol)
		{
			return new ExpressionEvaluationException(_invalidAssignmentException);
		}

		public static ExpressionEvaluationException MissingAssignment(string symbol, VariableReference variable)
		{
			return new ExpressionEvaluationException(_missingAssignmentException, variable);
		}

		public static ExpressionEvaluationException ReadOnlyAssignment(string symbol, VariableReference variable)
		{
			return new ExpressionEvaluationException(_missingAssignmentException, variable);
		}

		public static ExpressionEvaluationException MismatchedAssignment(string symbol, VariableReference variable, VariableType type)
		{
			return new ExpressionEvaluationException(_missingAssignmentException, variable, type);
		}

		public static ExpressionEvaluationException PrefixTypeMismatch(string symbol, VariableType type, VariableType expected)
		{
			return new ExpressionEvaluationException(_prefixTypeMismatch1Exception, symbol, type);
		}

		public static ExpressionEvaluationException PrefixTypeMismatch(string symbol, VariableType type, VariableType expected1, VariableType expected2)
		{
			return new ExpressionEvaluationException(_prefixTypeMismatch2Exception, symbol, type, expected1, expected2);
		}

		public static ExpressionEvaluationException PrefixTypeMismatch(string symbol, VariableType type, params VariableType[] expected)
		{
			var first = string.Join(",", expected.Take(expected.Length - 1));
			var last = expected[expected.Length - 1];

			return new ExpressionEvaluationException(_prefixTypeMismatchXException, symbol, type, first, last);
		}

		public static ExpressionEvaluationException InfixTypeMismatch(string symbol, VariableType left, VariableType right, VariableType expected)
		{
			return new ExpressionEvaluationException(_infixTypeMismatch1Exception, symbol, left, right, expected);
		}

		public static ExpressionEvaluationException InfixTypeMismatch(string symbol, VariableType left, VariableType right, VariableType expected1, VariableType expected2)
		{
			return new ExpressionEvaluationException(_infixTypeMismatch2Exception, symbol, left, right, expected1, expected2);
		}

		public static ExpressionEvaluationException InfixTypeMismatch(string symbol, VariableType left, VariableType right, params VariableType[] expected)
		{
			var first = string.Join(",", expected.Take(expected.Length - 1));
			var last = expected[expected.Length - 1];

			return new ExpressionEvaluationException(_infixTypeMismatchXException, symbol, left, right, first, last);
		}

		public static ExpressionEvaluationException ComparisonTypeMismatch(string symbol, VariableType left, VariableType right, VariableType expected)
		{
			return new ExpressionEvaluationException(_comparisonTypeMismatch1Exception, symbol, left, right, expected);
		}

		public static ExpressionEvaluationException ComparisonTypeMismatch(string symbol, VariableType left, VariableType right, VariableType expected1, VariableType expected2)
		{
			return new ExpressionEvaluationException(_comparisonTypeMismatch2Exception, symbol, left, right, expected1, expected2);
		}

		public static ExpressionEvaluationException ComparisonTypeMismatch(string symbol, VariableType left, VariableType right, params VariableType[] expected)
		{
			var first = string.Join(",", expected.Take(expected.Length - 1));
			var last = expected[expected.Length - 1];

			return new ExpressionEvaluationException(_comparisonTypeMismatchXException, symbol, left, right, first, last);
		}

		public static ExpressionEvaluationException DivideByZero(string symbol)
		{
			return new ExpressionEvaluationException(_divideByZeroException, symbol);
		}

		#endregion
	}

	public abstract class Operation
	{
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
