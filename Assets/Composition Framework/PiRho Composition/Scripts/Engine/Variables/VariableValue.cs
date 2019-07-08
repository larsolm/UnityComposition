using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
{
	public enum VariableType
	{
		Empty,
		Bool,
		Int,
		Float,
		Int2,
		Int3,
		IntRect,
		IntBounds,
		Vector2,
		Vector3,
		Vector4,
		Quaternion,
		Rect,
		Bounds,
		Color,
		String,
		Enum,
		Object,
		Store,
		List
	}

	public struct VariableValue
	{
		private VariableType _type;
		private ValueData _value;
		private object _reference;

		public VariableType Type => _type;
		public bool IsEmpty => _type == VariableType.Empty;
		public bool IsNull => HasReference && _reference == null;

		public bool HasValue => !HasString && !HasEnum && !HasReference;
		public bool HasString => _type == VariableType.String;
		public bool HasEnum => _type == VariableType.Enum;
		public bool HasReference => _type == VariableType.Object || _type == VariableType.Store || _type == VariableType.List;

		public bool HasObject => _reference is Object;
		public bool HasStore => _reference is IVariableStore;
		public bool HasList => _reference is IVariableList;

		public bool HasNumber => _type == VariableType.Int || _type == VariableType.Float;
		public bool HasNumber2 => _type == VariableType.Int2 || _type == VariableType.Vector2;
		public bool HasNumber3 => _type == VariableType.Int3 || _type == VariableType.Vector3 || HasNumber2;
		public bool HasNumber4 => _type == VariableType.Vector4 || HasNumber3;
		public bool HasRect => _type == VariableType.IntRect || _type == VariableType.Rect;
		public bool HasBounds => _type == VariableType.IntBounds || _type == VariableType.Bounds;
		
		public bool HasEnumType<Type>() where Type : Enum => HasEnumType(typeof(Type));
		public bool HasReferenceType<Type>() where Type : class => HasReferenceType(typeof(Type));
		public bool HasEnumType(Type type) => HasEnum && EnumType == type;
		public bool HasReferenceType(Type type) => HasReference && _reference != null && type.IsAssignableFrom(ReferenceType);

		#region Storage

		[StructLayout(LayoutKind.Explicit)]
		private struct ValueData
		{
			[FieldOffset(0)] public bool Bool;
			[FieldOffset(0)] public int Int;
			[FieldOffset(0)] public float Float;

			[FieldOffset(0)] public Vector2Int Int2;
			[FieldOffset(0)] public Vector3Int Int3;
			[FieldOffset(0)] public RectInt IntRect;
			[FieldOffset(0)] public BoundsInt IntBounds;

			[FieldOffset(0)] public Vector2 Vector2;
			[FieldOffset(0)] public Vector3 Vector3;
			[FieldOffset(0)] public Vector4 Vector4;
			[FieldOffset(0)] public Quaternion Quaternion;
			[FieldOffset(0)] public Rect Rect;
			[FieldOffset(0)] public Bounds Bounds;

			[FieldOffset(0)] public Color Color;
		}

		#endregion

		#region Creation

		public static VariableType GetType(Type type)
		{
			// Object takes precedence over Store which takes precedence over List

			if (type == typeof(bool)) return VariableType.Bool;
			else if (type == typeof(int)) return VariableType.Int;
			else if (type == typeof(float)) return VariableType.Float;
			else if (type == typeof(Vector2Int)) return VariableType.Int2;
			else if (type == typeof(Vector3Int)) return VariableType.Int3;
			else if (type == typeof(RectInt)) return VariableType.IntRect;
			else if (type == typeof(BoundsInt)) return VariableType.IntBounds;
			else if (type == typeof(Vector2)) return VariableType.Vector2;
			else if (type == typeof(Vector3)) return VariableType.Vector3;
			else if (type == typeof(Vector4)) return VariableType.Vector4;
			else if (type == typeof(Quaternion)) return VariableType.Quaternion;
			else if (type == typeof(Rect)) return VariableType.Rect;
			else if (type == typeof(Bounds)) return VariableType.Bounds;
			else if (type == typeof(Color)) return VariableType.Color;
			else if (type == typeof(string)) return VariableType.String;
			else if (type.IsEnum) return VariableType.Enum;
			else if (typeof(Object).IsAssignableFrom(type)) return VariableType.Object;
			else if (typeof(IVariableStore).IsAssignableFrom(type)) return VariableType.Store;
			else if (typeof(IVariableList).IsAssignableFrom(type)) return VariableType.List;
			else return VariableType.Empty;
		}

		public static VariableValue Empty => CreateReference(VariableType.Empty, null);
		public static VariableValue Create(bool value) => CreateValue(VariableType.Bool, new ValueData { Bool = value });
		public static VariableValue Create(int value) => CreateValue(VariableType.Int, new ValueData { Int = value });
		public static VariableValue Create(float value) => CreateValue(VariableType.Float, new ValueData { Float = value });
		public static VariableValue Create(Vector2Int value) => CreateValue(VariableType.Int2, new ValueData { Int2 = value });
		public static VariableValue Create(Vector3Int value) => CreateValue(VariableType.Int3, new ValueData { Int3 = value });
		public static VariableValue Create(RectInt value) => CreateValue(VariableType.IntRect, new ValueData { IntRect = value });
		public static VariableValue Create(BoundsInt value) => CreateValue(VariableType.IntBounds, new ValueData { IntBounds = value });
		public static VariableValue Create(Vector2 value) => CreateValue(VariableType.Vector2, new ValueData { Vector2 = value });
		public static VariableValue Create(Vector3 value) => CreateValue(VariableType.Vector3, new ValueData { Vector3 = value });
		public static VariableValue Create(Vector4 value) => CreateValue(VariableType.Vector4, new ValueData { Vector4 = value });
		public static VariableValue Create(Quaternion value) => CreateValue(VariableType.Quaternion, new ValueData { Quaternion = value });
		public static VariableValue Create(Rect value) => CreateValue(VariableType.Rect, new ValueData { Rect = value });
		public static VariableValue Create(Bounds value) => CreateValue(VariableType.Bounds, new ValueData { Bounds = value });
		public static VariableValue Create(Color value) => CreateValue(VariableType.Color, new ValueData { Color = value });
		public static VariableValue Create(string str) => CreateReference(VariableType.String, str ?? string.Empty);
		public static VariableValue Create(Enum e) => CreateReference(VariableType.Enum, e);
		public static VariableValue Create(Object obj) => CreateObject(obj);
		public static VariableValue Create(IVariableStore store) => CreateReference(VariableType.Store, store ?? new VariableStore());
		public static VariableValue Create(IVariableList list) => CreateReference(VariableType.List, list ?? new VariableList());

		public static VariableValue CreateValue<T>(T value)
		{
			// the compiler can't resolve the Create overload from a generic

			switch (value)
			{
				case bool b: return Create(b);
				case int i: return Create(i);
				case float f: return Create(f);
				case Vector2Int v: return Create(v);
				case Vector3Int v: return Create(v);
				case RectInt r: return Create(r);
				case BoundsInt b: return Create(b);
				case Vector2 v: return Create(v);
				case Vector3 v: return Create(v);
				case Vector4 v: return Create(v);
				case Quaternion q: return Create(q);
				case Rect r: return Create(r);
				case Bounds b: return Create(b);
				case Color c: return Create(c);
				case string s: return Create(s);
				default: return Empty;
			}
		}

		public static VariableValue CreateReference(object reference)
		{
			if (reference is string) return CreateReference(VariableType.String, reference);
			else if (reference is Enum) return CreateReference(VariableType.Enum, reference);
			else if (reference is Object obj) return CreateObject(obj);
			else if (reference is IVariableStore) return CreateReference(VariableType.Store, reference);
			else if (reference is IVariableList) return CreateReference(VariableType.List, reference);
			else return Empty;
		}

		public static VariableValue CreateAny(object obj)
		{
			var value = CreateValue(obj);
			return value.IsEmpty ? CreateReference(obj) : value;
		}

		private static VariableValue CreateValue(VariableType type, ValueData value)
		{
			return new VariableValue
			{
				_type = type,
				_value = value
			};
		}

		private static VariableValue CreateObject(Object reference)
		{
			// Make sure fake null unity objects are stored as real null. When loading, the fake null check will throw
			// an exception for fake nulls (though, not for valid objects) since it is happening on a background
			// thread.

			Object fixedReference;

			try { fixedReference = reference == null ? null : reference; }
			catch { fixedReference = null; }

			return CreateReference(VariableType.Object, fixedReference); 
		}

		private static VariableValue CreateReference(VariableType type, object reference)
		{
			return new VariableValue
			{
				_type = type,
				_reference = reference
			};
		}

		#endregion

		#region Access

		public bool Bool => _value.Bool;
		public int Int => _value.Int;
		public float Float => _value.Float;
		public Vector2Int Int2 => _value.Int2;
		public Vector3Int Int3 => _value.Int3;
		public RectInt IntRect => _value.IntRect;
		public BoundsInt IntBounds => _value.IntBounds;
		public Vector2 Vector2 => _value.Vector2;
		public Vector3 Vector3 => _value.Vector3;
		public Vector4 Vector4 => _value.Vector4;
		public Quaternion Quaternion => _value.Quaternion;
		public Rect Rect => _value.Rect;
		public Bounds Bounds => _value.Bounds;
		public Color Color => _value.Color;
		public string String => _reference as string;
		public Enum Enum => _reference as Enum;
		public Object Object => _reference as Object;
		public IVariableStore Store => _reference as IVariableStore;
		public IVariableList List => _reference as IVariableList;

		public float Number => TryGetFloat(out var number) ? number : 0.0f;
		public Vector2 Number2 => TryGetVector2(out var vector) ? vector : Vector2.zero;
		public Vector3 Number3 => TryGetVector3(out var vector) ? vector : Vector3.zero;
		public Vector4 Number4 => TryGetVector4(out var vector) ? vector : Vector4.zero;
		public Rect NumberRect => TryGetRect(out var rect) ? rect : new Rect(0, 0, 0, 0);
		public Bounds NumberBounds => TryGetBounds(out var bounds) ? bounds : new Bounds(Vector3.zero, Vector3.zero);
		public object Reference => _reference;

		public Type EnumType => HasEnum ? _reference.GetType() : null;
		public Type ReferenceType => HasReference && _reference != null ? _reference.GetType() : null;

		public override string ToString() => VariableHandler.ToString(this);

		private VariableType GetListType()
		{
			var list = List;
			var type = list.Count > 0 ? list.GetVariable(0).Type : VariableType.Empty;

			for (var i = 1; i < list.Count; i++)
			{
				if (list.GetVariable(i).Type != type)
					return VariableType.Empty;
			}

			return type;
		}

		public object GetBoxedValue()
		{
			switch (_type)
			{
				case VariableType.Bool: return _value.Bool;
				case VariableType.Int: return _value.Int;
				case VariableType.Float: return _value.Float;
				case VariableType.Int2: return _value.Int2;
				case VariableType.Int3: return _value.Int3;
				case VariableType.IntRect: return _value.IntRect;
				case VariableType.IntBounds: return _value.IntBounds;
				case VariableType.Vector2: return _value.Vector2;
				case VariableType.Vector3: return _value.Vector3;
				case VariableType.Vector4: return _value.Vector4;
				case VariableType.Quaternion: return _value.Quaternion;
				case VariableType.Rect: return _value.Rect;
				case VariableType.Bounds: return _value.Bounds;
				case VariableType.Color: return _value.Color;
				default: return _reference;
			}
		}

		#endregion

		#region Casting

		// ACCESSOR				ADDITIONAL VALID TYPES
		// TryGetFloat			Int
		// TryGetInt3			Int2 (z = 0)
		// TryGetIntBounds		IntRect (z = 0, d = 0)
		// TryGetVector2		Int2
		// TryGetVector3		Vector2 (z = 0), Int3, Int2 (z = 0)
		// TryGetVector4		Vector3 (w = 1), Vector2 (z = 0, w = 1), Int3 (w = 1), Int2 (z = 0, w = 1)
		// TryGetRect			IntRect
		// TryGetBounds			IntBounds, Rect (z = 0, d = 0), IntRect(z = 0, d = 0)
		// TryGetObject			valid whenever _reference is an Object
		// TryGetStore			valid whenever _reference is an IVariableStore
		// TryGetList			valid whenever _reference is an IVariableList
		// TryGetReference		valid whenever _reference is a T

		public bool TryGetBool(out bool value)
		{
			if (_type == VariableType.Bool)
			{
				value = _value.Bool;
				return true;
			}
			else
			{
				value = false;
				return false;
			}
		}

		public bool TryGetInt(out int value)
		{
			if (_type == VariableType.Int)
			{
				value = _value.Int;
				return true;
			}
			else
			{
				value = 0;
				return false;
			}
		}

		public bool TryGetFloat(out float value)
		{
			if (_type == VariableType.Float)
			{
				value = _value.Float;
				return true;
			}
			else if (_type == VariableType.Int)
			{
				value = _value.Int;
				return true;
			}
			else
			{
				value = 0.0f;
				return false;
			}
		}

		public bool TryGetInt2(out Vector2Int value)
		{
			if (_type == VariableType.Int2)
			{
				value = _value.Int2;
				return true;
			}
			else
			{
				value = Vector2Int.zero;
				return false;
			}
		}

		public bool TryGetInt3(out Vector3Int value)
		{
			if (_type == VariableType.Int3)
			{
				value = _value.Int3;
				return true;
			}
			else if (TryGetInt2(out var int2))
			{
				value = new Vector3Int(int2.x, int2.y, 0);
				return true;
			}
			else
			{
				value = Vector3Int.zero;
				return false;
			}
		}

		public bool TryGetIntRect(out RectInt value)
		{
			if (_type == VariableType.IntRect)
			{
				value = _value.IntRect;
				return true;
			}
			else
			{
				value = new RectInt(0, 0, 0, 0);
				return false;
			}
		}

		public bool TryGetIntBounds(out BoundsInt value)
		{
			if (_type == VariableType.IntBounds)
			{
				value = _value.IntBounds;
				return true;
			}
			else if (TryGetIntRect(out var rect))
			{
				value = new BoundsInt(rect.xMin, rect.yMin, 0, rect.width, rect.height, 0);
				return false;
			}
			else
			{
				value = new BoundsInt(0, 0, 0, 0, 0, 0);
				return false;
			}
		}

		public bool TryGetVector2(out Vector2 value)
		{
			if (_type == VariableType.Vector2)
			{
				value = _value.Vector3;
				return true;
			}
			else if (TryGetInt2(out var i2))
			{
				value = new Vector2(i2.x, i2.y);
				return true;
			}
			else
			{
				value = new Vector2();
				return false;
			}
		}

		public bool TryGetVector3(out Vector3 value)
		{
			if (_type == VariableType.Vector3)
			{
				value = _value.Vector3;
				return true;
			}
			else if (TryGetInt3(out var i3))
			{
				value = new Vector3(i3.x, i3.y, i3.z);
				return true;
			}
			else if (TryGetVector2(out Vector2 v2))
			{
				value = new Vector3(v2.x, v2.y, 0.0f);
				return true;
			}
			else
			{
				value = new Vector3();
				return false;
			}
		}

		public bool TryGetVector4(out Vector4 value)
		{
			if (_type == VariableType.Vector4)
			{
				value = _value.Vector4;
				return true;
			}
			else if (TryGetVector3(out Vector3 v3))
			{
				value = new Vector4(v3.x, v3.y, v3.z, 1.0f);
				return true;
			}
			else
			{
				value = Vector4.zero;
				return false;
			}
		}

		public bool TryGetQuaternion(out Quaternion value)
		{
			if (_type == VariableType.Quaternion)
			{
				value = _value.Quaternion;
				return true;
			}
			else
			{
				value = Quaternion.identity;
				return false;
			}
		}

		public bool TryGetRect(out Rect value)
		{
			if (_type == VariableType.Rect)
			{
				value = _value.Rect;
				return true;
			}
			else
			{
				value = new Rect(0, 0, 0, 0);
				return false;
			}
		}

		public bool TryGetBounds(out Bounds value)
		{
			if (_type == VariableType.Bounds)
			{
				value = _value.Bounds;
				return true;
			}
			else if (TryGetIntBounds(out var bounds))
			{
				value = new Bounds(bounds.min, bounds.max);
				return true;
			}
			else if (TryGetRect(out Rect rect))
			{
				value = new Bounds(rect.min, rect.max);
				return true;
			}
			else
			{
				value = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
		}

		public bool TryGetColor(out Color value)
		{
			if (_type == VariableType.Color)
			{
				value = _value.Color;
				return true;
			}
			else
			{
				value = Color.white;
				return false;
			}
		}

		public bool TryGetString(out string s)
		{
			if (_type == VariableType.String)
			{
				s = (string)_reference;
				return true;
			}
			else
			{
				s = string.Empty;
				return false;
			}
		}

		public bool TryGetEnum<EnumType>(out EnumType value) where EnumType : Enum
		{
			if (HasEnumType<EnumType>())
			{
				value = (EnumType)Enum;
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		public bool TryGetObject(out Object obj)
		{
			obj = _reference as Object;
			return obj != null;
		}

		public bool TryGetStore(out IVariableStore store)
		{
			store = _reference as IVariableStore;
			return store != null;
		}

		public bool TryGetList(out IVariableList list)
		{
			list = _reference as IVariableList;
			return list != null;
		}

		public bool TryGetReference<T>(out T t) where T : class
		{
			t = _reference as T;
			return t != null;
		}

		#endregion
	}
}
