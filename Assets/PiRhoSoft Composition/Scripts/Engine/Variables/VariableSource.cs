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
		public StringVariableSource() => Value = string.Empty;
		public StringVariableSource(string defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector2VariableSource : VariableSource<Vector2>
	{
		public Vector2VariableSource() => Value = Vector2.zero;
		public Vector2VariableSource(Vector2 defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector2IntVariableSource : VariableSource<Vector2Int>
	{
		public Vector2IntVariableSource() => Value = Vector2Int.zero;
		public Vector2IntVariableSource(Vector2Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector3VariableSource : VariableSource<Vector3>
	{
		public Vector3VariableSource() => Value = Vector2.zero;
		public Vector3VariableSource(Vector3 defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector3IntVariableSource : VariableSource<Vector3Int>
	{
		public Vector3IntVariableSource() => Value = Vector3Int.zero;
		public Vector3IntVariableSource(Vector3Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector4VariableSource : VariableSource<Vector4>
	{
		public Vector4VariableSource() => Value = Vector4.zero;
		public Vector4VariableSource(Vector4 defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class QuaternionVariableSource : VariableSource<Quaternion>
	{
		public QuaternionVariableSource() => Value = Quaternion.identity;
		public QuaternionVariableSource(Quaternion defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class ColorVariableSource : VariableSource<Color>
	{
		public ColorVariableSource() => Value = Color.black;
		public ColorVariableSource(Color defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class RectVariableSource : VariableSource<Rect>
	{
		public RectVariableSource() => Value = new Rect(Vector2.zero, Vector2.one);
		public RectVariableSource(Rect defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class RectIntVariableSource : VariableSource<RectInt>
	{
		public RectIntVariableSource() => Value = new RectInt(Vector2Int.zero, Vector2Int.one);
		public RectIntVariableSource(RectInt defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class BoundsVariableSource : VariableSource<Bounds>
	{
		public BoundsVariableSource() => Value = new Bounds(Vector3.zero, Vector3.one);
		public BoundsVariableSource(Bounds defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class BoundsIntVariableSource : VariableSource<BoundsInt>
	{
		public BoundsIntVariableSource() => Value = new BoundsInt(Vector3Int.zero, Vector3Int.one);
		public BoundsIntVariableSource(BoundsInt defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class ObjectVariableSource : VariableSource<Object>
	{
	}

	[Serializable]
	public class GameObjectVariableSource : VariableSource<GameObject>
	{
	}

	[Serializable]
	public class VariableValueSource : VariableSource<VariableValue>
	{
		public VariableDefinition Definition;

		public VariableValueSource() { Value = VariableValue.Empty; Definition = VariableDefinition.Create(string.Empty, VariableType.Empty); }
		public VariableValueSource(VariableType type, VariableDefinition definition) { Value = VariableValue.Create(type); Definition = definition; }
	}
}
