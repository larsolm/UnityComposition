using System;
using System.Collections.Generic;
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
		public abstract string Write(IList<Object> objects);
		public abstract bool Read(string data, IList<Object> objects);

		public abstract bool IsValid(VariableValue value);
	}
}
