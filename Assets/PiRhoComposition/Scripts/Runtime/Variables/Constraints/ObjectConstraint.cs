using PiRhoSoft.Utilities;
using System;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class ObjectConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Object;

		private Type _objectType;

		public Type ObjectType
		{
			get => _objectType;
			set => _objectType = value ?? typeof(Object);
		}

		public ObjectConstraint()
		{
			ObjectType = null;
		}

		public ObjectConstraint(Type type)
		{
			ObjectType = type;
		}

		public override string ToString()
		{
			return _objectType.Name;
		}

		public override Variable Generate()
		{
			return Variable.Object(null);
		}

		public override bool IsValid(Variable variable)
		{
			return variable.IsNullObject || variable.HasObject(ObjectType);
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.SaveType(ObjectType);
		}

		public override void Load(SerializedDataReader reader)
		{
			ObjectType = reader.LoadType();
		}
	}
}
