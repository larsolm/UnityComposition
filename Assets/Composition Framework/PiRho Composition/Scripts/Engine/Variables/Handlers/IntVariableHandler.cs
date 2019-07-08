using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
{
	internal class IntVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			if (constraint is IntVariableConstraint intConstraint)
				return VariableValue.Create(intConstraint.Minimum);
			else
				return VariableValue.Create(0);
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Int);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Int);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var i = reader.ReadInt32();
			return VariableValue.Create(i);
		}

		protected internal override VariableValue Add_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int + i);
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Int + f);
			else if (right.Type == VariableType.String)
				return VariableValue.Create(left.Int + right.String);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Subtract_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int - i);
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Int - f);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Multiply_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create(left.Int * i);
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Int * f);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Divide_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(left.Int / i);
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create(left.Int / f);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Modulo_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i) && i != 0)
				return VariableValue.Create(left.Int % i);
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Exponent_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return VariableValue.Create((float)Math.Pow(left.Int, i));
			else if (right.TryGetFloat(out var f))
				return VariableValue.Create((float)Math.Pow(left.Int, f));
			else
				return VariableValue.Empty;
		}

		protected internal override VariableValue Negate_(VariableValue value)
		{
			return VariableValue.Create(-value.Int);
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return left.Int == i;
			else
				return null;
		}

		protected internal override int? Compare_(VariableValue left, VariableValue right)
		{
			if (right.TryGetInt(out var i))
				return left.Int.CompareTo(i);
			else
				return null;
		}
	}
}
