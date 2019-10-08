using PiRhoSoft.Utilities;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class VariableDefinition
	{
		public VariableDefinition()
		{
			Name = string.Empty;
			Type = VariableType.Empty;
		}

		public VariableDefinition(string name)
		{
			Name = name;
			Type = VariableType.Empty;
		}

		public VariableDefinition(string name, VariableType type)
		{
			Name = name;
			Type = type;
		}

		public VariableDefinition(string name, VariableConstraint constraint)
		{
			Name = name;
			Constraint = constraint;
		}

		public string Name;
		[SerializeField] private VariableType _type;
		private VariableConstraint _constraint;

		public string Description
		{
			get
			{
				if (Constraint == null)
					return Type.ToString();
				else
					return string.Format($"{Type}({Constraint})");
			}
		}

		public VariableType Type
		{
			get => _type;
			set
			{
				_type = value;
				_constraint = VariableConstraint.Create(value);
			}
		}

		public VariableConstraint Constraint
		{
			get => _constraint;
			set
			{
				_type = value?.Type ?? VariableType.Empty;
				_constraint = value;
			}
		}

		public Variable Generate()
		{
			return _constraint != null
				? _constraint.Generate()
				: Variable.Create(Type);
		}

		public bool IsValid(Variable variable)
		{
			return _constraint != null
				? _constraint.IsValid(variable)
				: variable.Is(Type);
		}

		[SerializeField] private SerializedData _constraintData = new SerializedData();
		//void ISerializationCallbackReceiver.OnBeforeSerialize() => _constraintData.SaveClass(_constraint, 1);
		//void ISerializationCallbackReceiver.OnAfterDeserialize() => _constraintData.LoadClass(out _constraint);
	}

	[Serializable]
	public class VariableDefinitionList : SerializedList<VariableDefinition>
	{
		private const string _duplicateDefinitionWarning = "(VDLDD) failed to add definition '{0}' with type '{1}': the definition has already been added with type '{2}'";

		public new void Add(VariableDefinition definition)
		{
			Add(definition, null);
		}

		public void Add(VariableDefinition definition, Object context)
		{
			var existing = this.FirstOrDefault(d => d.Name == definition.Name);

			if (existing != null)
				Merge(definition, existing, context);
			else
				base.Add(definition);
		}

		private void Merge(VariableDefinition from, VariableDefinition into, Object context)
		{
			if (into.Type == VariableType.Empty)
			{
				into.Type = from.Type;
			}
			else if (from.Type != VariableType.Empty && into.Type != from.Type)
			{
				Debug.LogFormat(context, _duplicateDefinitionWarning, from.Type);
				return;
			}

			// TODO: ideally there would be a warning if both have different and non-default constraints - not sure
			// the best way to do that

			if (into.Constraint == null)
				into.Constraint = from.Constraint;
		}
	}

	[Serializable]
	public abstract class VariableConstraint : ISerializableData
	{
		public abstract VariableType Type { get; }
		public abstract Variable Generate();
		public abstract bool IsValid(Variable value);
		public abstract void Save(BinaryWriter writer, SerializedData data);
		public abstract void Load(BinaryReader reader, SerializedData data);

		public static VariableConstraint Create(VariableType type)
		{
			switch (type)
			{
				case VariableType.Int: return new IntConstraint();
				case VariableType.Float: return new FloatConstraint();
				case VariableType.Enum: return new EnumConstraint();
				case VariableType.String: return new StringConstraint();
				case VariableType.List: return new ListConstraint();
				case VariableType.Dictionary: return new DictionaryConstraint();
				case VariableType.Asset: return new AssetConstraint();
				case VariableType.Object: return new ObjectConstraint();
				default: return null;
			}
		}
	}
}