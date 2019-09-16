﻿using System.Text;

namespace PiRhoSoft.Composition
{
	internal class TypeOperation : Operation
	{
		public VariableType Type { get; private set; }

		public TypeOperation(VariableType type)
		{
			Type = type;
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
		}

		public override Variable Evaluate(IVariableCollection variables)
		{
			return Variable.Create(Type);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Type);
		}
	}
}