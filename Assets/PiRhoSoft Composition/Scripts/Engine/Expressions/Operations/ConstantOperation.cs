using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ConstantOperation : Operation
	{
		private VariableValue _value;

		public ConstantOperation(VariableValue value)
		{
			_value = value;
		}

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return _value;
		}

		public override void ToString(StringBuilder builder)
		{
			switch (_value.Type)
			{
				case VariableType.Object:
				{
					if (_value.IsNull)
						builder.Append(ExpressionLexer.NullConstant);
					else
						VariableHandler.ToString(_value, builder);

					break;
				}
				case VariableType.Bool:
				{
					if (_value.Bool)
						builder.Append(ExpressionLexer.TrueConstant);
					else
						builder.Append(ExpressionLexer.FalseConstant);

					break;
				}
				case VariableType.Float:
				{
					if (_value.Float == Mathf.PI)
						builder.Append(ExpressionLexer.PiConstant);
					else if (_value.Float == Mathf.Deg2Rad)
						builder.Append(ExpressionLexer.Deg2RadConstant);
					else if (_value.Float == Mathf.Rad2Deg)
						builder.Append(ExpressionLexer.Rad2DegConstant);
					else
						VariableHandler.ToString(_value, builder);

					break;
				}
				default:
				{
					VariableHandler.ToString(_value, builder);
					break;
				}
			}
		}
	}
}
