using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class VariableDefinitionList : SerializedList<VariableDefinition> { }
	[Serializable] public class ValueDefinitionList : SerializedList<ValueDefinition> { }

	[Serializable]
	public struct VariableDefinition
	{
		public string Name;
		public ValueDefinition Definition;
	}

	[Serializable]
	public struct ValueDefinition : ISerializationCallbackReceiver
	{
		private const string _invalidInitializerError = "(CVDII) Failed to initialize variable on '{0}': the definition specifies type '{1}' but the initializer returned type '{2}'";

		[SerializeField] private VariableType _type;
		[SerializeField] private string _constraint;
		[SerializeField] private List<Object> _objects;
		[SerializeField] private string _tag;
		[SerializeField] private Expression _initializer;

		[SerializeField] private bool _isTypeLocked;
		[SerializeField] private bool _isConstraintLocked;

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

				Debug.LogErrorFormat(_invalidInitializerError, variables, Type, value.Type);
			}

			return VariableHandler.CreateDefault(Type, Constraint);
		}

		public bool IsValid(VariableValue value)
		{
			return _type == value.Type && (Constraint == null || Constraint.IsValid(value));
		}

		#region Creation

		public static ValueDefinition Create(VariableType type)
		{
			return new ValueDefinition
			{
				_type = type,
				_isTypeLocked = type != VariableType.Empty,
				_isConstraintLocked = false
			};
		}

		public static ValueDefinition Create(int minimum, int maximum)
		{
			return new ValueDefinition
			{
				_type = VariableType.Int,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static ValueDefinition Create(float minimum, float maximum)
		{
			return new ValueDefinition
			{
				_type = VariableType.Float,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static ValueDefinition Create(string[] values)
		{
			return new ValueDefinition
			{
				_type = VariableType.String,
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static ValueDefinition Create<T>() where T : Object
		{
			return Create(typeof(T));
		}

		public static ValueDefinition Create(Type type)
		{
			return new ValueDefinition
			{
				_type = VariableValue.GetType(type),
				_isTypeLocked = true,
				_isConstraintLocked = true
			};
		}

		public static ValueDefinition Create(VariableType type, VariableConstraint constraint)
		{
			return new ValueDefinition
			{
				_type = type,
				_isTypeLocked = type != VariableType.Empty,
				_isConstraintLocked = constraint != null,
				Constraint = constraint
			};
		}

		public static ValueDefinition Create(VariableType type, VariableConstraint constraint, string tag, Expression initializer, bool isTypeLocked, bool isConstraintLocked)
		{
			return new ValueDefinition
			{
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
			if (!string.IsNullOrEmpty(_constraint))
				Constraint = VariableHandler.LoadConstraint(ref _constraint, ref _objects);
		}

		public void OnBeforeSerialize()
		{
			if (Constraint != null)
				_constraint = VariableHandler.SaveConstraint(Type, Constraint, ref _objects);
		}

		#endregion
	}
}
