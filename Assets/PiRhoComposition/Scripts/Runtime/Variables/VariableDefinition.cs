using PiRhoSoft.Utilities;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class VariableDefinition : ISerializationCallbackReceiver
	{
		public const string TypeProperty = nameof(_type);
		public const string ConstraintProperty = nameof(_constraintData);

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

		[SerializeField] private SerializedDataItem _constraintData = new SerializedDataItem();
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			using (var writer = new SerializedDataWriter(_constraintData))
				writer.SaveInstance(_constraint);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			using (var reader = new SerializedDataReader(_constraintData))
				_constraint = reader.LoadInstance<VariableConstraint>();
		}
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
}