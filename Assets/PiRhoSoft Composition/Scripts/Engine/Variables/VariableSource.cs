using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum VariableSourceType
	{
		Value,
		Reference
	}

	public abstract class VariableSource
	{
		[Tooltip("Whether the source points to a variable reference or an actual value")]
		[EnumButtons]
		public VariableSourceType Type = VariableSourceType.Value;

		[Tooltip("The variable reference to lookup the value from")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Reference)]
		public VariableReference Reference = new VariableReference();

		public abstract void GetInputs(List<VariableDefinition> inputs);
	}

	public abstract class VariableSource<T> : VariableSource
	{
		[Tooltip("The specific value of the source")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Value)]
		public T Value;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (Type == VariableSourceType.Reference && InstructionStore.IsInput(Reference))
			{
				var type = VariableValue.GetType(typeof(T));
				inputs.Add(VariableDefinition.Create(Reference.RootName, type));
			}
		}
	}

	[Serializable]
	public class BooleanVariableSource : VariableSource<bool>
	{
		public BooleanVariableSource() => Value = false;
		public BooleanVariableSource(bool defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class IntegerVariableSource : VariableSource<int>
	{
		public IntegerVariableSource() => Value = 0;
		public IntegerVariableSource(int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class NumberVariableSource : VariableSource<float>
	{
		public NumberVariableSource() => Value = 0.0f;
		public NumberVariableSource(float defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class StringVariableSource : VariableSource<string>
	{
		public StringVariableSource() => Value = "";
		public StringVariableSource(string defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class GameObjectVariableSource : VariableSource<GameObject>
	{
	}
}
