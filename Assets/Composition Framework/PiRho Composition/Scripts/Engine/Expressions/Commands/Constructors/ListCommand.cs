using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	internal class ListCommand : ICommand
	{
		public Variable Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
			{
				return Variable.List(new VariableList());
			}
			else if (parameters.Count == 1)
			{
				var count = parameters[0].Evaluate(variables);

				if (!count.IsInt)
					throw CommandEvaluationException.WrongParameterType(name, 0, count.Type, VariableType.Int);

				return Variable.List(new VariableList(count.AsInt));
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0, 1);
		}
	}
}
