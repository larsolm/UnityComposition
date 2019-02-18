using System;
using PiRhoSoft.UtilityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ExponentOperator : InfixOperation
	{
		public static VariableValue Raise(ref VariableValue left, ref VariableValue right)
		{
			switch (left.Type)
			{
				case VariableType.Integer:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create(MathHelper.IntExponent(left.Integer, right.Integer));
						case VariableType.Number: return VariableValue.Create((float)Math.Pow(left.Integer, right.Number));
					}

					break;
				}
				case VariableType.Number:
				{
					switch (right.Type)
					{
						case VariableType.Integer: return VariableValue.Create((float)Math.Pow(left.Number, right.Integer));
						case VariableType.Number: return VariableValue.Create((float)Math.Pow(left.Number, right.Number));
					}

					break;
				}
			}

			throw new ExpressionEvaluationException(MismatchedMathType2Exception, '^', left.Type, right.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return Raise(ref left, ref right);
		}
	}
}
