using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
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

		public VariableConstraintAttribute(List<string> values)
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
		public static VariableConstraint Create(VariableType type)
		{
			switch (type)
			{
				case VariableType.Int: return new IntVariableConstraint();
				case VariableType.Float: return new FloatVariableConstraint();
				case VariableType.String: return new StringVariableConstraint();
				case VariableType.Enum: return new EnumVariableConstraint();
				case VariableType.Object: return new ObjectVariableConstraint();
				case VariableType.Store: return new StoreVariableConstraint();
				case VariableType.List: return new ListVariableConstraint();
				default: return null;
			}
		}

		protected internal abstract void Write(BinaryWriter writer, IList<Object> objects);
		protected internal abstract void Read(BinaryReader reader, IList<Object> objects, short version);

		public abstract bool IsValid(VariableValue value);
	}
}
