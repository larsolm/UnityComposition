using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ConstantCommand : ICommand
	{
		public VariableValue Value { get; private set; }

		public ConstantCommand(VariableValue value) => Value = value;

		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count != 0)
				throw new CommandEvaluationException(name, Command.TooManyParametersException, parameters.Count, parameters.Count == 1 ? "" : "s", 0);

			return Value;
		}
	}
}
