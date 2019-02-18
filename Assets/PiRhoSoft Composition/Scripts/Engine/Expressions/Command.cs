using System;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class CommandEvaluationException : Exception
	{
		public string Command;

		public CommandEvaluationException(string command, string error) : base(error) => Command = command;
		public CommandEvaluationException(string command, string errorFormat, params object[] arguments) : this(command, string.Format(errorFormat, arguments)) { }
	}

	public abstract class Command
	{
		public const string WrongParameterCountException = "the Command was passed {0} parameter{1} but expected {2}";
		public const string WrongParameterRangeException = "the Command was passed {0} parameter{1} but expected between {2} and {3}";
		public const string TooFewParametersException = "the Command was passed {0} parameter{1} but expected at least {2}";
		public const string TooManyParametersException = "the Command was passed {0} parameter{1} but expected at most {2}";
		public const string WrongParameterType1Exception = "the Command was passed a {0} for parameter {1} but expected a {2}";
		public const string WrongParameterType2Exception = "the Command was passed a {0} for parameter {1} but expected a {2} or {3}";
		public const string InvalidRangeException = "the min value ({0}) should be less than the max value ({1})";

		public abstract VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters);
	}
}
