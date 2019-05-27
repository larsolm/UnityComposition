using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class ConstantOperation : Operation
	{
		public VariableValue Value { get; private set; }

		public ConstantOperation(VariableValue value)
		{
			Value = value;
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return Value;
		}

		public override void ToString(StringBuilder builder)
		{
			switch (Value.Type)
			{
				case VariableType.Object:
				{
					if (Value.IsNull)
						builder.Append(ExpressionLexer.NullConstant);
					else
						VariableHandler.ToString(Value, builder);

					break;
				}
				case VariableType.Bool:
				{
					if (Value.Bool)
						builder.Append(ExpressionLexer.TrueConstant);
					else
						builder.Append(ExpressionLexer.FalseConstant);

					break;
				}
				case VariableType.Float:
				{
					if (Value.Float == Mathf.PI)
						builder.Append(ExpressionLexer.PiConstant);
					else if (Value.Float == Mathf.Deg2Rad)
						builder.Append(ExpressionLexer.Deg2RadConstant);
					else if (Value.Float == Mathf.Rad2Deg)
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
