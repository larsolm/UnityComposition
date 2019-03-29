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

		public abstract void GetInputs(IList<VariableDefinition> inputs);
	}

	public abstract class VariableSource<T> : VariableSource
	{
		[Tooltip("The specific value of the source")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Value)]
		public T Value;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (Type == VariableSourceType.Reference && InstructionStore.IsInput(Reference))
			{
				var type = VariableValue.GetType(typeof(T));
				inputs.Add(new VariableDefinition { Name = Reference.RootName, Definition = ValueDefinition.Create(type) });
			}
		}
	}

	[Serializable]
	public class BoolVariableSource : VariableSource<bool>
	{
		public BoolVariableSource() => Value = false;
		public BoolVariableSource(bool defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class IntVariableSource : VariableSource<int>
	{
		public IntVariableSource() => Value = 0;
		public IntVariableSource(int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class FloatVariableSource : VariableSource<float>
	{
		public FloatVariableSource() => Value = 0.0f;
		public FloatVariableSource(float defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Int2VariableSource : VariableSource<Vector2Int>
	{
		public Int2VariableSource() => Value = Vector2Int.zero;
		public Int2VariableSource(Vector2Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Int3VariableSource : VariableSource<Vector3Int>
	{
		public Int3VariableSource() => Value = Vector3Int.zero;
		public Int3VariableSource(Vector3Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class IntRectVariableSource : VariableSource<RectInt>
	{
		public IntRectVariableSource() => Value = new RectInt(Vector2Int.zero, Vector2Int.one);
		public IntRectVariableSource(RectInt defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class IntBoundsVariableSource : VariableSource<BoundsInt>
	{
		public IntBoundsVariableSource() => Value = new BoundsInt(Vector3Int.zero, Vector3Int.one);
		public IntBoundsVariableSource(BoundsInt defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector2VariableSource : VariableSource<Vector2>
	{
		public Vector2VariableSource() => Value = Vector2.zero;
		public Vector2VariableSource(Vector2 defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector3VariableSource : VariableSource<Vector3>
	{
		public Vector3VariableSource() => Value = Vector2.zero;
		public Vector3VariableSource(Vector3 defaultValue) => Value = defaultValue;
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
	public class RectVariableSource : VariableSource<Rect>
	{
		public RectVariableSource() => Value = new Rect(Vector2.zero, Vector2.one);
		public RectVariableSource(Rect defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class BoundsVariableSource : VariableSource<Bounds>
	{
		public BoundsVariableSource() => Value = new Bounds(Vector3.zero, Vector3.one);
		public BoundsVariableSource(Bounds defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class ColorVariableSource : VariableSource<Color>
	{
		public ColorVariableSource() => Value = Color.black;
		public ColorVariableSource(Color defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class StringVariableSource : VariableSource<string>
	{
		public StringVariableSource() => Value = string.Empty;
		public StringVariableSource(string defaultValue) => Value = defaultValue;
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
	public class StoreVariableSource : VariableSource<IVariableStore>
	{
	}

	[Serializable]
	public class ListVariableSource : VariableSource<IVariableList>
	{
	}

	[Serializable]
	public class VariableValueSource : VariableSource<VariableValue>, ISerializationCallbackReceiver
	{
		public ValueDefinition Definition;

		[SerializeField] private string _data;
		[SerializeField] private List<Object> _objects;

		public VariableValueSource() { Value = VariableValue.Empty; Definition = ValueDefinition.Create(VariableType.Empty); }
		public VariableValueSource(VariableType type, ValueDefinition definition) { Value = definition.Generate(null); Definition = definition; }

		public void OnBeforeSerialize()
		{
			VariableValue.Save(Value, ref _data, ref _objects);
		}

		public void OnAfterDeserialize()
		{
			VariableValue.Load(ref Value, ref _data, ref _objects);
		}
	}
}
