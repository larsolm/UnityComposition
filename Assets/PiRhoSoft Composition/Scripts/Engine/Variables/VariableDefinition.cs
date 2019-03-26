using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class VariableDefinitionList : SerializedList<VariableDefinition> { }

	[Serializable]
	public struct VariableDefinition : ISerializationCallbackReceiver
	{
		private const string _invalidInitializerError = "(CVDII) Failed to initialize variable '{0}': the definition specifies type {1} but the initializer returned type {2}";

		[SerializeField] private string _name;
		[SerializeField] private VariableType _type;
		[SerializeField] private string _constraint;
		[SerializeField] public string _tag;
		[SerializeField] public Expression _initializer;

		[SerializeField] private bool _isTypeLocked;
		[SerializeField] private bool _isConstraintLocked;

		public string Name => _name;
		public VariableType Type => _type;
		public VariableConstraint Constraint { get; private set; }
		public string Tag => _tag;
		public Expression Initializer => _initializer;

		public bool IsTypeLocked => _isTypeLocked;
		public bool IsConstraintLocked => _isConstraintLocked;

		public VariableValue Generate(IVariableStore variables)
		{
			if (_initializer != null && _initializer.IsValid && variables != null)
			{
				// if variables isn't an object there isn't a context that makes sense anyway, so null is fine
				var value = _initializer.Execute(variables as Object, variables);

				if (value.Type == Type)
					return value;

				Debug.LogErrorFormat(_invalidInitializerError, Name, Type, value.Type);
			}

			return VariableHandler.Get(Type).CreateDefault(Constraint);
		}

		public bool IsValid(VariableValue value)
		{
			return _type == value.Type && (Constraint == null || Constraint.IsValid(value));
		}

		#region Creation

		public static VariableDefinition Create(string name, VariableType type)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_isTypeLocked = type != VariableType.Empty,
				_isConstraintLocked = false
			};
		}

		public static VariableDefinition Create(string name, int minimum, int maximum)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Int,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static VariableDefinition Create(string name, float minimum, float maximum)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Float,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static VariableDefinition Create(string name, string[] values)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.String,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static VariableDefinition Create<T>(string name) where T : Object
		{
			return Create(name, typeof(T));
		}

		public static VariableDefinition Create(string name, Type type)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableValue.GetType(type),
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static VariableDefinition Create(string name, VariableType type, VariableConstraint constraint)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_isTypeLocked = type != VariableType.Empty,
				_isConstraintLocked = constraint != null,
				Constraint = constraint
			};
		}

		public static VariableDefinition Create(string name, VariableType type, VariableConstraint constraint, string tag, Expression initializer, bool isTypeLocked, bool isConstraintLocked)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_isTypeLocked = isTypeLocked,
				_isConstraintLocked = isConstraintLocked,
				Constraint = constraint,
				_tag = tag,
				_initializer = initializer
			};
		}

		#endregion

		#region ISerializationCallbackReceiver

		public void OnAfterDeserialize()
		{
			Constraint = VariableHandler.Get(Type).CreateConstraint(_constraint);
			_constraint = null;
		}

		public void OnBeforeSerialize()
		{
			if (Constraint != null)
				_constraint = Constraint.Write();
		}

		#endregion
	}
}
