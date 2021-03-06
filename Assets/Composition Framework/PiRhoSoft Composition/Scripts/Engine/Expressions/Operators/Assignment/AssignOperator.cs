﻿namespace PiRhoSoft.CompositionEngine
{
	internal class AssignOperator : AssignmentOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			return Assign(variables, result);
		}
	}
}
