using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class ConstantOperation : Operation
	{
		public Variable Value { get; private set; }

		public ConstantOperation(Variable value)
		{
			Value = value;
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
		}

		public override Variable Evaluate(IVariableCollection variables)
		{
			return Value;
		}

		public override void ToString(StringBuilder builder)
		{
			switch (Value.Type)
			{
				case VariableType.Object:
				{
					if (Value.IsNullObject)
						builder.Append(ExpressionLexer.NullConstant);
					else
						VariableHandler.ToString(Value, builder);

					break;
				}
				case VariableType.Bool:
				{
					if (Value.AsBool)
						builder.Append(ExpressionLexer.TrueConstant);
					else
						builder.Append(ExpressionLexer.FalseConstant);

					break;
				}
				case VariableType.Float:
				{
					if (Value.AsFloat == Mathf.PI)
						builder.Append(ExpressionLexer.PiConstant);
					else if (Value.AsFloat == Mathf.Deg2Rad)
						builder.Append(ExpressionLexer.Deg2RadConstant);
					else if (Value.AsFloat == Mathf.Rad2Deg)
						builder.Append(ExpressionLexer.Rad2DegConstant);
					else
						VariableHandler.ToString(Value, builder);

					break;
				}
				default:
				{
					VariableHandler.ToString(Value, builder);
					break;
				}
			}
		}
	}
}
