using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public struct VariableDefinition
	{
		private const string _invalidInitializerError = "(CVDII) Failed to initialize variable '{0}': the definition specifies type {1} but the initializer returned type {2}";

		public const string NotSaved = "Always (not saved)";
		public const string Saved = "Always (saved)";

		[SerializeField] private string _name;
		[SerializeField] private VariableType _type;
		[SerializeField] private string _availability;
		[SerializeField] private Expression _initializer;

		// These constraints are not checked at runtime - they are only for providing information to the editor so it
		// can show friendlier controls.

		[SerializeField] private bool _useRangeConstraint;
		[SerializeField] private float _minimumConstraint;
		[SerializeField] private float _maximumConstraint;
		[SerializeField] private string _typeConstraint;

		public string Name => _name;
		public VariableType Type => _type;
		public string Availability => _availability;
		public Expression Initializer => _initializer;

		public bool UseRangeConstraint => _useRangeConstraint;
		public float MinimumConstraint => _minimumConstraint;
		public float MaximumConstraint => _maximumConstraint;
		public string TypeConstraint => _typeConstraint;

		public static VariableDefinition Create(string name, VariableType type, string availability = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_availability = availability,
				_initializer = initializer
			};
		}

		public static VariableDefinition Create(string name, int minimum, int maximum, string availability = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Integer,
				_availability = availability,
				_initializer = initializer,
				_useRangeConstraint = true,
				_minimumConstraint = minimum,
				_maximumConstraint = maximum
			};
		}

		public static VariableDefinition Create(string name, float minimum, float maximum, string availability = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Number,
				_availability = availability,
				_initializer = initializer,
				_useRangeConstraint = true,
				_minimumConstraint = minimum,
				_maximumConstraint = maximum
			};
		}

		public static VariableDefinition Create(string name, string values, string availability = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.String,
				_availability = availability,
				_initializer = initializer,
				_typeConstraint = values
			};
		}

		public static VariableDefinition Create<T>(string name, string availability = "", Expression initializer = null) where T : Object
		{
			return new VariableDefinition
			{
				_name = name,
				_type = VariableType.Object,
				_availability = availability,
				_initializer = initializer,
				_typeConstraint = typeof(T).AssemblyQualifiedName
			};
		}

		public static VariableDefinition Create(string name, VariableType type, bool constrainRange, float minimum, float maximum, string typeConstraint, string availability = "", Expression initializer = null)
		{
			return new VariableDefinition
			{
				_name = name,
				_type = type,
				_availability = availability,
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
				var value = Initializer.Execute(variables);

				if (value.Type == Type)
					return Variable.Create(Name, value);

				Debug.LogErrorFormat(_invalidInitializerError, Name, Type, value.Type);
			}

			return Variable.Create(Name, VariableValue.Create(Type));
		}
	}
}
