using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is IntVariableConstraint intConstraint)
				return VariableValue.Create(intConstraint.Minimum);
			else
				return VariableValue.Create(0);
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Int);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var i = reader.ReadInt32();
			return VariableValue.Create(i);
		}

		protected override VariableConstraint CreateConstraint()
		{
			return new IntVariableConstraint();
		}

		protected override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int + i);
			else if (right.Type == VariableType.String)
				return VariableValue.Create(left.Int + right.String);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int - i);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int * i);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(left.Int / i);
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Int / f);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Modulo_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(left.Int % i);
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Exponent_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create((float)Math.Pow(left.Int, i));
			else
				return VariableValue.Empty;
		}

		protected override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(-value.Int);
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return left.Int == i;
			else
				return null;
		}

		protected override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return left.Int.CompareTo(i);
			else
				return null;
		}
	}
}
