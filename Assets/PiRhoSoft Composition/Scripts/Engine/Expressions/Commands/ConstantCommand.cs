using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ConstantCommand : ICommand
	{
		public VariableValue Value { get; private set; }

		public ConstantCommand(VariableValue value) => Value = value;

		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return Value;
			else
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
