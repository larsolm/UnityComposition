using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	internal class FloatVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is FloatVariableConstraint floatConstraint)
				return VariableValue.Create(floatConstraint.Minimum);
			else
				return VariableValue.Create(0.0f);
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Float);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Float);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var f = reader.ReadSingle();
			return VariableValue.Create(f);
		}

		protected internal override VariableConstraint CreateConstraint()
		{
			return new FloatVariableConstraint();
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float + number);
			else if (right.Type == VariableType.String)
				return VariableValue.Create(left.Float + right.String);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float - number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float * number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float / number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Modulo_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create(left.Float % number);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Exponent_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return VariableValue.Create((float)Math.Pow(left.Float, number));
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(-value.Float);
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return left.Float == number;
			else
				return null;
		}

		protected internal override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.TryGetFloat(out var number))
				return left.Float.CompareTo(number);
			else
				return null;
		}
	}
}
