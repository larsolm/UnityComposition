using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	internal class ListCommand : ICommand
	{
		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
			{
				return VariableValue.Create(new VariableList());
			}
			else if (parameters.Count == 1)
			{
				var count = parameters[0].Evaluate(variables);

				if (count.Type != VariableType.Int)
					throw CommandEvaluationException.WrongParameterType(name, 0, count.Type, VariableType.Int);

				return VariableValue.Create(new VariableList(count.Int));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0, 1);
		}
	}
}
