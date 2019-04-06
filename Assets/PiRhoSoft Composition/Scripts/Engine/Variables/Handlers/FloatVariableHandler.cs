using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class FloatVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is FloatVariableConstraint floatConstraint)
				return VariableValue.Create(floatConstraint.Minimum);
			else
				return VariableValue.Create(0.0f);
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Float);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Float);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var f = reader.ReadSingle();
			return VariableValue.Create(f);
		}

		protected override VariableConstraint CreateConstraint()
		{
			return new FloatVariableConstraint();
		}

		protected override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float + number);
			else if (right.Type == VariableType.String)
				return VariableValue.Create(left.Float + right.String);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float - number);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float * number);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float / number);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Modulo_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float % number);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Exponent_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create((float)Math.Pow(left.Float, number));
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(-value.Float);
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return left.Float == number;
			else
				return null;
		}

		protected override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return left.Float.CompareTo(number);
			else
				return null;
		}
	}
}
