using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public enum VariableSourceType
	{
		Value,
		Reference
	}

	public abstract class VariableSource
	{
		public VariableSourceType Type = VariableSourceType.Value;
		[Conditional(nameof(Type), (int)VariableSourceType.Value)] public VariableReference Reference = new VariableReference();

		public void GetInputs(IList<VariableDefinition> inputs)
		{
			if (Type == VariableSourceType.Reference && GraphStore.IsInput(Reference))
			{
				var definition = GetInputDefinition();
				definition.Name = Reference.RootName;
				inputs.Add(definition);
			}
		}

		protected abstract VariableDefinition GetInputDefinition();
	}

	public abstract class VariableSource<T> : VariableSource
	{
		public T Value;

		protected override VariableDefinition GetInputDefinition()
		{
			var type = Variable.GetType(typeof(T));
			return new VariableDefinition(string.Empty, type);
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
	public class Vector2IntVariableSource : VariableSource<Vector2Int>
	{
		public Vector2IntVariableSource() => Value = Vector2Int.zero;
		public Vector2IntVariableSource(Vector2Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class Vector3IntVariableSource : VariableSource<Vector3Int>
	{
		public Vector3IntVariableSource() => Value = Vector3Int.zero;
		public Vector3IntVariableSource(Vector3Int defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class RectIntVariableSource : VariableSource<RectInt>
	{
		public RectIntVariableSource() => Value = new RectInt(Vector2Int.zero, Vector2Int.one);
		public RectIntVariableSource(RectInt defaultValue) => Value = defaultValue;
	}

	[Serializable]
	public class BoundsIntVariableSource : VariableSource<BoundsInt>
	{
		public BoundsIntVariableSource() => Value = new BoundsInt(Vector3Int.zero, Vector3Int.one);
		public BoundsIntVariableSource(BoundsInt defaultValue) => Value = defaultValue;
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
	public class ListVariableSource : VariableSource<VariableList>
	{
		public ListVariableSource() => Value = new VariableList();
	}

	[Serializable]
	public class DictionaryVariableSource : VariableSource<VariableDictionary>
	{
		public DictionaryVariableSource() => Value = new VariableDictionary();
	}

	[Serializable]
	public class AssetVariableSource : VariableSource<AssetReference>
	{
		public AssetVariableSource() => Value = new AssetReference();
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
	public class VariableValueSource : VariableSource
	{
		public VariableDefinition Definition = new VariableDefinition();

		[Tooltip("The specific value of the source")]
		[Conditional(nameof(Type), (int)VariableSourceType.Value)]
		public VariableValue Value = new VariableValue();

		public VariableValueSource() { }
		public VariableValueSource(VariableType type, VariableDefinition definition) { Definition = definition; Value.Variable = definition.Generate(); }

		protected override VariableDefinition GetInputDefinition() => Definition;
	}
}