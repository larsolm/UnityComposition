using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class BoolVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Create(false);
		protected override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(value.Bool);

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(value.Bool);
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var b = reader.ReadBoolean();
			return VariableValue.Create(b);
		}

		protected override VariableValue And_(VariableValue left, VariableValue right)
		{
			return right.Type == VariableType.Bool ? VariableValue.Create(left.Bool && right.Bool) : VariableValue.Empty;
		}

		protected override VariableValue Or_(VariableValue left, VariableValue right)
		{
			return right.Type == VariableType.Bool ? VariableValue.Create(left.Bool || right.Bool) : VariableValue.Empty;
		}

		protected override VariableValue Not_(VariableValue value)
		{
			return VariableValue.Create(!value.Bool);
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.Type == VariableType.Bool)
				return left.Bool == right.Bool;
			else
				return null;
		}
	}
}
