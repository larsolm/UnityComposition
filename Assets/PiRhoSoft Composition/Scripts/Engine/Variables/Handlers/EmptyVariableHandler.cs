using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class EmptyVariableHandler : VariableHandler
	{
		public const string EmptyText = "(empty)";

		protected internal override VariableValue CreateDefault_(VariableConstraint constraint) => VariableValue.Empty;
		protected internal override void ToString_(VariableValue value, StringBuilder builder) => builder.Append(EmptyText);
		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects) { }
		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version) => VariableValue.Empty;

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
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
