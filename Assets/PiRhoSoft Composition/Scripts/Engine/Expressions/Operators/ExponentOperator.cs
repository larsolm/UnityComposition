using System;
using PiRhoSoft.UtilityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ExponentOperator : InfixOperation
	{
		public static VariableValue Raise(string symbol, ref VariableValue left, ref VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Int:
				{
					switch (right.Type)
					{
						case VariableType.Int: return VariableValue.Create(MathHelper.IntExponent(left.Int, right.Int));
						case VariableType.Float: return VariableValue.Create((float)Math.Pow(left.Int, right.Float));
					}

					break;
				}
				case VariableType.Float:
				{
					switch (right.Type)
					{
						case VariableType.Int: return VariableValue.Create((float)Math.Pow(left.Float, right.Int));
						case VariableType.Float: return VariableValue.Create((float)Math.Pow(left.Float, right.Float));
					}

					break;
				}
			}

			throw ExpressionEvaluationException.InfixTypeMismatch(symbol, left.Type, right.Type, VariableType.Int, VariableType.Float);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Raise(Symbol, ref left, ref right);
		}
	}
}
