using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class EmptyVariableHandler : VariableHandler
	{
		public const string EmptyText = "(empty)";

		protected override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Empty;
		protected override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(EmptyText);
		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects) { }
		protected override VariableValue Read_(BinaryReader reader, List<Object> objects) => VariableValue.Empty;

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.IsEmpty)
				return true;
			else if (right.HasReference)
				return right.IsNull;
			else
				return null;
		}
	}
}
