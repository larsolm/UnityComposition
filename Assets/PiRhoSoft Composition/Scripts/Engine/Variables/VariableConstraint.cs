using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class VariableConstraintAttribute : Attribute
	{
		public ValueDefinition Definition { get; private set; }

		public VariableConstraintAttribute(VariableType type)
		{
			Definition = ValueDefinition.Create(type);
		}

		public VariableConstraintAttribute(int minimum, int maximum)
		{
			Definition = ValueDefinition.Create(minimum, maximum);
		}

		public VariableConstraintAttribute(float minimum, float maximum)
		{
			Definition = ValueDefinition.Create(minimum, maximum);
		}

		public VariableConstraintAttribute(string[] values)
		{
			Definition = ValueDefinition.Create(values);
		}

		public VariableConstraintAttribute(Type type)
		{
			Definition = ValueDefinition.Create(type);
		}
	}

	public abstract class VariableConstraint
	{
		protected internal abstract void Write(BinaryWriter writer, IList<Object> objects);
		protected internal abstract void Read(BinaryReader reader, IList<Object> objects, short version);

		public abstract bool IsValid(VariableValue value);
	}
}
