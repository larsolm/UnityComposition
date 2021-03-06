﻿using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	internal abstract class MemberOperator : InfixOperation
	{
		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			if (Left is MemberOperator)
				Left.GetInputs(inputs, source);
		}
	}
}
