using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class LerpCommand : Command
	{
		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 3)
			{
				var a = parameters[0].Evaluate(variables);
				var b = parameters[1].Evaluate(variables);
				var t = parameters[2].Evaluate(variables);

				if (a.Type != VariableType.Integer && a.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, a.Type, 0, VariableType.Number);

				if (b.Type != VariableType.Integer && b.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, b.Type, 1, VariableType.Number);

				if (t.Type != VariableType.Integer && t.Type != VariableType.Number)
					throw new CommandEvaluationException(name, WrongParameterType1Exception, t.Type, 2, VariableType.Number);

				return VariableValue.Create(Mathf.Lerp(a.Number, b.Number, t.Number));
			}
			else
			{
				throw new CommandEvaluationException(name, WrongParameterCountException, parameters.Count, parameters.Count == 1 ? "" : "s", 3);
			}
		}
	}
}
