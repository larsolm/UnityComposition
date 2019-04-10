using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class VariableConstraintAttribute : Attribute
	{
		private ValueDefinition _definition;

		public VariableConstraintAttribute(VariableType type)
		{
			_definition = ValueDefinition.Create(type);
		}

		public VariableConstraintAttribute(int minimum, int maximum)
		{
			_definition = ValueDefinition.Create(minimum, maximum);
		}

		public VariableConstraintAttribute(float minimum, float maximum)
		{
			_definition = ValueDefinition.Create(minimum, maximum);
		}

		public VariableConstraintAttribute(string[] values)
		{
			_definition = ValueDefinition.Create(values);
		}

		public VariableConstraintAttribute(Type type)
		{
			_definition = ValueDefinition.Create(type);
		}

		public ValueDefinition GetDefinition()
		{
			return ValueDefinition.Create(_definition.Type, _definition.Constraint);
		}
	}

	public abstract class VariableConstraint
	{
		public abstract string Write(IList<Object> objects);
		public abstract bool Read(string data, IList<Object> objects);

		public abstract bool IsValid(VariableValue value);
	}
}
