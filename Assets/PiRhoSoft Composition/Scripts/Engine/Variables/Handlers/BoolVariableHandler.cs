using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class BoolVariableHandler : VariableHandler
	{
		protected internal override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(false);
		protected internal override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.Bool);

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Bool);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var b = reader.ReadBoolean();
			return VariableValue.Create(b);
		}

		protected internal override VariableValue And_(VariableValue left, VariableValue right)
		{
			return right.Type == VariableType.Bool ? VariableValue.Create(left.Bool && right.Bool) : VariableValue.Empty;
		}

		protected internal override VariableValue Or_(VariableValue left, VariableValue right)
		{
			return right.Type == VariableType.Bool ? VariableValue.Create(left.Bool || right.Bool) : VariableValue.Empty;
		}

		protected internal override VariableValue Not_(VariableValue value)
		{
			return VariableValue.Create(!value.Bool);
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.Bool)
				return left.Bool == right.Bool;
			else
				return null;
		}
	}
}
