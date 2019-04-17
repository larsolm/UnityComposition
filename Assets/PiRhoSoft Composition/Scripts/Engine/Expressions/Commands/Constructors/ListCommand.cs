using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ListCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
				return VariableValue.Create(new VariableList());

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0);
		}
	}
}
