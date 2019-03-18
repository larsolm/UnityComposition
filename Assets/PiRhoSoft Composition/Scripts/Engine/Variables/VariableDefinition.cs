using System;
using UnityEngine;
using Object = UnityEngine.Object;

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

		public VariableConstraintAttribute(string values)
		{
			_definition = VariableDefinition.Create(string.Empty, values);
		}

		public VariableConstraintAttribute(Type type)
		{
			_definition = VariableDefinition.Create(string.Empty, type);
		}

		public VariableDefinition GetDefinition(string name)
		{
			return VariableDefinition.Create(name, _definition.Type, _definition.UseRangeConstraint, _definition.MinimumConstraint, _definition.MaximumConstraint, _definition.TypeConstraint);
		}
	}

	[Serializable]
	public struct VariableDefinition
	{
		private const string _invalidInitializerError = "(CVDII) Failed to initialize variable '{0}': the definition specifies type {1} but the initializer returned type {2}";

		public const string NotSaved = "Always (not saved)";
		public const string Saved = "Always (saved)";

		[SerializeField] private string _name;
		[SerializeField] private VariableType _type;
		[SerializeField] private string _tag;
		[SerializeField] private Expression _initializer;

		// These constraints are not checked at runtime - they are only for providing information to the editor so it
		// can show friendlier controls.

		[SerializeField] private bool _useRangeConstraint;
		[SerializeField] private float _minimumConstraint;
		[SerializeField] private float _maximumConstraint;
		[SerializeField] private string _typeConstraint;

		public string Name => _name;
		public VariableType Type => _type;
		public string Tag => _tag;
		public Expression Initializer => _initializer;

		public bool UseRangeConstraint => _useRangeConstraint;
		public float MinimumConstraint => _minimumConstraint;
		public float MaximumConstraint => _maximumConstraint;
		public string TypeConstraint => _typeConstraint;

		public static VariableDefinition Create(string name, VariableType type, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_tag = tag,
				_initializer = initializer
			};
		}

		public static VariableDefinition Create(string name, int minimum, int maximum, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Integer,
				_tag = tag,
				_initializer = initializer,
				_useRangeConstraint = true,
				_minimumConstraint = minimum,
				_maximumConstraint = maximum
			};
		}

		public static VariableDefinition Create(string name, float minimum, float maximum, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Number,
				_tag = tag,
				_initializer = initializer,
				_useRangeConstraint = true,
				_minimumConstraint = minimum,
				_maximumConstraint = maximum
			};
		}

		public static VariableDefinition Create(string name, string values, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.String,
				_tag = tag,
				_initializer = initializer,
				_typeConstraint = values
			};
		}

		public static VariableDefinition Create<T>(string name, string tag = "", Expression initializer = null) where T : Object
		{
			return Create(name, typeof(T), tag, initializer);
		}

		public static VariableDefinition Create(string name, Type type, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Object,
				_tag = tag,
				_initializer = initializer,
				_typeConstraint = type.AssemblyQualifiedName
			};
		}

		public static VariableDefinition Create(string name, VariableType type, bool constrainRange, float minimum, float maximum, string typeConstraint, string tag = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_tag = tag,
				_initializer = initializer,
				_useRangeConstraint = constrainRange,
				_minimumConstraint = minimum,
				_maximumConstraint = maximum,
				_typeConstraint = typeConstraint
			};
		}

		public Variable Generate(IVariableStore variables)
		{
			if (Initializer != null && Initializer.IsValid && variables != null)
			{
				// if variables isn't an object there isn't a context that makes sense anyway, so null is fine
				var value = Initializer.Execute(variables as Object, variables);

				if (value.Type == Type)
					return Variable.Create(Name, value);

				Debug.LogErrorFormat(_invalidInitializerError, Name, Type, value.Type);
			}

			return Variable.Create(Name, VariableValue.Create(Type));
		}
	}
}
