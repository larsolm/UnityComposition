using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public enum VariableSourceType
	{
		Value,
		Reference
	}

	public abstract class VariableSource<T>
	{
		protected const string _referenceMissingError = "(CVSRM) failed to get value from source: the variable '{0}' could not be found";
		protected const string _referenceInvalidError = "(CVSRI) failed to get value from source: the variable '{0}' is of type {1} and should be of type {2}";

		[Tooltip("Whether the source points to a variable reference or an actual value")]
		[EnumButtons]
		public VariableSourceType Type = VariableSourceType.Value;

		[Tooltip("The variable reference to lookup the value from")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Reference)]
		public VariableReference Reference = new VariableReference();

		[Tooltip("The specific value of the source")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Value)]
		public T Value;

		protected abstract bool TryGetValue(VariableValue variable, out T Value);

		public void GetInputs(List<VariableDefinition> inputs)
		{
			if (Type == VariableSourceType.Reference && InstructionStore.IsInput(Reference))
			{
				var type = VariableValue.GetType(typeof(T));
				inputs.Add(VariableDefinition.Create(Reference.RootName, type));
			}
		}

		public bool TryGetValue(IVariableStore variables, Object context, out T value)
		{
			switch (Type)
			{
				case VariableSourceType.Value:
				{
					value = Value;
					return true;
				}
				case VariableSourceType.Reference:
				{
					var variable = Reference.GetValue(variables);

					if (variable.Type == VariableType.Empty)
					{
						value = default;
						Debug.LogErrorFormat(context, _referenceMissingError, Reference);
						return false;
					}

					if (!TryGetValue(variable, out value))
					{
						var actualType = GetTypeName(variable);
						var expectedType = GetTypeName(VariableValue.GetType(typeof(T)));
						Debug.LogErrorFormat(context, _referenceInvalidError, Reference, actualType, expectedType);
						return false;
					}

					return true;
				}
				default:
				{
					value = default;
					return false;
				}
			}
		}

		private string GetTypeName(VariableValue variable)
		{
			if (variable.Type == VariableType.Object)
				return variable.Object.GetType().Name;
			else
				return variable.Type.ToString();
		}

		private string GetTypeName(VariableType type)
		{
			if (type == VariableType.Object)
				return typeof(T).Name;
			else
				return type.ToString();
		}
	}

	[Serializable]
	public class BooleanVariableSource : VariableSource<bool>
	{
		public BooleanVariableSource() => Value = false;
		public BooleanVariableSource(bool defaultValue) => Value = defaultValue;

		protected override bool TryGetValue(VariableValue variable, out bool value)
		{
			return variable.TryGetBoolean(out value);
		}
	}

	[Serializable]
	public class IntegerVariableSource : VariableSource<int>
	{
		public IntegerVariableSource() => Value = 0;
		public IntegerVariableSource(int defaultValue) => Value = defaultValue;

		protected override bool TryGetValue(VariableValue variable, out int value)
		{
			return variable.TryGetInteger(out value);
		}
	}

	[Serializable]
	public class NumberVariableSource : VariableSource<float>
	{
		public NumberVariableSource() => Value = 0.0f;
		public NumberVariableSource(float defaultValue) => Value = defaultValue;

		protected override bool TryGetValue(VariableValue variable, out float value)
		{
			return variable.TryGetNumber(out value);
		}
	}

	[Serializable]
	public class StringVariableSource : VariableSource<string>
	{
		public StringVariableSource() => Value = "";
		public StringVariableSource(string defaultValue) => Value = defaultValue;

		protected override bool TryGetValue(VariableValue variable, out string value)
		{
			return variable.TryGetString(out value);
		}
	}

	[Serializable]
	public class ObjectVariableSource<ObjectType> : VariableSource<ObjectType> where ObjectType : ScriptableObject
	{
		protected override bool TryGetValue(VariableValue variable, out ObjectType value)
		{
			return variable.TryGetObject(out value);
		}
	}
}
