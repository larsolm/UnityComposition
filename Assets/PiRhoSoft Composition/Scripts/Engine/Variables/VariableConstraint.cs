using System;

namespace PiRhoSoft.CompositionEngine
{
	public class VariableConstraintAttribute : Attribute
	{
		private VariableDefinition _definition;

		public VariableConstraintAttribute(VariableType type)
		{
			_definition = VariableDefinition.Create(string.Empty, type);
		}

		public VariableConstraintAttribute(int minimum, int maximum)
		{
			_definition = VariableDefinition.Create(string.Empty, minimum, maximum);
		}

		public VariableConstraintAttribute(float minimum, float maximum)
		{
			_definition = VariableDefinition.Create(string.Empty, minimum, maximum);
		}

		public VariableConstraintAttribute(string[] values)
		{
			_definition = VariableDefinition.Create(string.Empty, values);
		}

		public VariableConstraintAttribute(Type type)
		{
			_definition = VariableDefinition.Create(string.Empty, type);
		}

		public VariableDefinition GetDefinition(string name)
		{
			return VariableDefinition.Create(name, _definition.Type, _definition.Constraint);
		}
	}

	public abstract class VariableConstraint
	{
		public abstract string Write();
		public abstract bool Read(string data);

		public abstract bool IsValid(VariableValue value);
	}
}
