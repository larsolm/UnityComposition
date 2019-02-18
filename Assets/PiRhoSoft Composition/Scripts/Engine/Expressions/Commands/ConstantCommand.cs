using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ConstantCommand : Command
	{
		public VariableValue Value { get; private set; }

		public ConstantCommand(VariableValue value) => Value = value;

		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count != 0)
				throw new CommandEvaluationException(name, TooManyParametersException, parameters.Count, parameters.Count == 1 ? "" : "s", 0);

			return Value;
		}
	}
}
