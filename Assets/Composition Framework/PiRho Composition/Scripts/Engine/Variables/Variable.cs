using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public enum VariableType
	{
		Empty,
		Bool,
		Int,
		Float,
		Vector2Int,
		Vector3Int,
		RectInt,
		BoundsInt,
		Vector2,
		Vector3,
		Vector4,
		Quaternion,
		Rect,
		Bounds,
		Color,
		Enum,
		String,
		List,
		Dictionary,
		Object
	}

	public struct Variable
	{
		public VariableType Type { get; private set; }
		private ValueData _value;
		private object _reference;

		public override string ToString() => VariableHandler.ToString(this);

		#region ValueData

		[StructLayout(LayoutKind.Explicit)]
		private struct ValueData
		{
			[FieldOffset(0)] public bool Bool;
			[FieldOffset(0)] public int Int;
			[FieldOffset(0)] public float Float;

			[FieldOffset(0)] public Vector2Int Vector2Int;
			[FieldOffset(0)] public Vector3Int Vector3Int;
			[FieldOffset(0)] public RectInt RectInt;
			[FieldOffset(0)] public BoundsInt BoundsInt;

			[FieldOffset(0)] public Vector2 Vector2;
			[FieldOffset(0)] public Vector3 Vector3;
			[FieldOffset(0)] public Vector4 Vector4;
			[FieldOffset(0)] public Quaternion Quaternion;
			[FieldOffset(0)] public Rect Rect;
			[FieldOffset(0)] public Bounds Bounds;

			[FieldOffset(0)] public Color Color;
		}

		private static Variable CreateValue(VariableType type, ValueData value)
		{
			return new Variable
			{
				Type = type,
				_value = value
			};
		}

		public bool IsValueType
		{
			get
			{
				switch (Type)
				{
					case VariableType.Empty: return false;
					case VariableType.Bool: return true;
					case VariableType.Int: return true;
					case VariableType.Float: return true;
					case VariableType.Vector2Int: return true;
					case VariableType.Vector3Int: return true;
					case VariableType.RectInt: return true;
					case VariableType.BoundsInt: return true;
					case VariableType.Vector2: return true;
					case VariableType.Vector3: return true;
					case VariableType.Vector4: return true;
					case VariableType.Quaternion: return true;
					case VariableType.Rect: return true;
					case VariableType.Bounds: return true;
					case VariableType.Color: return true;
					case VariableType.Enum: return false;
					case VariableType.String: return false;
					case VariableType.List: return false;
					case VariableType.Dictionary: return false;
					case VariableType.Object: return false;
					default: return false;
				}
			}
		}

		#endregion

		#region References

		public static Variable Unbox(object obj)
		{
			switch (obj)
			{
				case bool b: return Bool(b);
				case int i: return Int(i);
				case float f: return Float(f);
				case Vector2Int v: return Vector2Int(v);
				case Vector3Int v: return Vector3Int(v);
				case RectInt r: return RectInt(r);
				case BoundsInt b: return BoundsInt(b);
				case Vector2 v: return Vector2(v);
				case Vector3 v: return Vector3(v);
				case Vector4 v: return Vector4(v);
				case Quaternion q: return Quaternion(q);
				case Rect r: return Rect(r);
				case Bounds b: return Bounds(b);
				case Color c: return Color(c);
				case Enum e: return Enum(e);
				case string s: return String(s);
				case IVariableList l: return List(l);
				case IVariableDictionary d: return Dictionary(d);
				default: return Object(obj);
			}
		}

		public object Box()
		{
			switch (Type)
			{
				case VariableType.Empty: return null;
				case VariableType.Bool: return AsBool;
				case VariableType.Int: return AsInt;
				case VariableType.Float: return AsFloat;
				case VariableType.Vector2Int: return AsVector2Int;
				case VariableType.Vector3Int: return AsVector3Int;
				case VariableType.RectInt: return AsRectInt;
				case VariableType.BoundsInt: return AsBoundsInt;
				case VariableType.Vector2: return AsVector2;
				case VariableType.Vector3: return AsVector3;
				case VariableType.Vector4: return AsVector4;
				case VariableType.Quaternion: return AsQuaternion;
				case VariableType.Rect: return AsRect;
				case VariableType.Bounds: return AsBounds;
				case VariableType.Color: return AsColor;
				case VariableType.Enum: return AsEnum;
				case VariableType.String: return AsString;
				case VariableType.List: return AsList;
				case VariableType.Dictionary: return AsDictionary;
				case VariableType.Object: return AsObject;
				default: return null;
			}
		}

		private static Variable CreateReference(VariableType type, object reference)
		{
			return new Variable
			{
				Type = type,
				_reference = reference
			};
		}

		#endregion

		#region VariableType

		public static Variable Create(VariableType type)
		{
			switch (type)
			{
				case VariableType.Empty: return Empty;
				case VariableType.Bool: return Bool(false);
				case VariableType.Int: return Int(0);
				case VariableType.Float: return Float(0.0f);
				case VariableType.Vector2Int: return Vector2Int(UnityEngine.Vector2Int.zero);
				case VariableType.Vector3Int: return Vector3Int(UnityEngine.Vector3Int.zero);
				case VariableType.RectInt: return RectInt(new RectInt());
				case VariableType.BoundsInt: return BoundsInt(new BoundsInt());
				case VariableType.Vector2: return Vector2(UnityEngine.Vector2.zero);
				case VariableType.Vector3: return Vector3(UnityEngine.Vector3.zero);
				case VariableType.Vector4: return Vector4(UnityEngine.Vector4.zero);
				case VariableType.Quaternion: return Quaternion(UnityEngine.Quaternion.identity);
				case VariableType.Rect: return Rect(new Rect());
				case VariableType.Bounds: return Bounds(new Bounds());
				case VariableType.Color: return Color(UnityEngine.Color.black);
				case VariableType.Enum: return Enum(null);
				case VariableType.String: return String(null);
				case VariableType.List: return List(null);
				case VariableType.Dictionary: return Dictionary(null);
				case VariableType.Object: return Object(null);
				default: return Empty;
			}
		}

		public bool Is(VariableType type)
		{
			switch (type)
			{
				case VariableType.Empty: return IsEmpty;
				case VariableType.Bool: return IsBool;
				case VariableType.Int: return IsInt;
				case VariableType.Float: return IsFloat;
				case VariableType.Vector2Int: return IsVector2Int;
				case VariableType.Vector3Int: return IsVector3Int;
				case VariableType.RectInt: return IsRectInt;
				case VariableType.BoundsInt: return IsBoundsInt;
				case VariableType.Vector2: return IsVector2;
				case VariableType.Vector3: return IsVector3;
				case VariableType.Vector4: return IsVector4;
				case VariableType.Quaternion: return IsQuaternion;
				case VariableType.Rect: return IsRect;
				case VariableType.Bounds: return IsBounds;
				case VariableType.Color: return IsColor;
				case VariableType.Enum: return IsEnum;
				case VariableType.String: return IsString;
				case VariableType.List: return IsList;
				case VariableType.Dictionary: return IsDictionary;
				case VariableType.Object: return IsObject;
				default: return false;
			}
		}

		#endregion

		#region Generic

		public static Variable Create<T>(T value)
		{
			switch (value)
			{
				case bool b: return Bool(b);
				case int i: return Int(i);
				case float f: return Float(f);
				case Vector2Int v: return Vector2Int(v);
				case Vector3Int v: return Vector3Int(v);
				case RectInt r: return RectInt(r);
				case BoundsInt b: return BoundsInt(b);
				case Vector2 v: return Vector2(v);
				case Vector3 v: return Vector3(v);
				case Vector4 v: return Vector4(v);
				case Quaternion q: return Quaternion(q);
				case Rect r: return Rect(r);
				case Bounds b: return Bounds(b);
				case Color c: return Color(c);
				case Enum e: return Enum(e);
				case string s: return String(s);
				case IVariableList l: return List(l);
				case IVariableDictionary s: return Dictionary(s);
				default: return Object(value);
			}
		}

		public static VariableType GetType(Type type)
		{
			if (type == typeof(bool)) return VariableType.Bool;
			else if (type == typeof(int)) return VariableType.Int;
			else if (type == typeof(float)) return VariableType.Float;
			else if (type == typeof(Vector2Int)) return VariableType.Vector2Int;
			else if (type == typeof(Vector3Int)) return VariableType.Vector3Int;
			else if (type == typeof(RectInt)) return VariableType.RectInt;
			else if (type == typeof(BoundsInt)) return VariableType.BoundsInt;
			else if (type == typeof(Vector2)) return VariableType.Vector2;
			else if (type == typeof(Vector3)) return VariableType.Vector3;
			else if (type == typeof(Vector4)) return VariableType.Vector4;
			else if (type == typeof(Quaternion)) return VariableType.Quaternion;
			else if (type == typeof(Rect)) return VariableType.Rect;
			else if (type == typeof(Bounds)) return VariableType.Bounds;
			else if (type == typeof(Color)) return VariableType.Color;
			else if (type.IsEnum) return VariableType.Enum;
			else if (type == typeof(string)) return VariableType.String;
			else if (typeof(IVariableList).IsAssignableFrom(type)) return VariableType.List;
			else if (typeof(IVariableDictionary).IsAssignableFrom(type)) return VariableType.Dictionary;
			else return VariableType.Object;
		}

		public bool Is<T>()
		{
			var type = typeof(T);

			if (type == typeof(bool)) return IsBool;
			else if (type == typeof(int)) return IsInt;
			else if (type == typeof(float)) return IsFloat;
			else if (type == typeof(Vector2Int)) return IsVector2Int;
			else if (type == typeof(Vector3Int)) return IsVector3Int;
			else if (type == typeof(RectInt)) return IsRectInt;
			else if (type == typeof(BoundsInt)) return IsBoundsInt;
			else if (type == typeof(Vector2)) return IsVector2;
			else if (type == typeof(Vector3)) return IsVector3;
			else if (type == typeof(Vector4)) return IsVector4;
			else if (type == typeof(Quaternion)) return IsQuaternion;
			else if (type == typeof(Rect)) return IsRect;
			else if (type == typeof(Bounds)) return IsBounds;
			else if (type == typeof(Color)) return IsColor;
			else if (type.IsEnum) return HasEnum(type);
			else if (type == typeof(string)) return IsString;
			else if (typeof(IVariableList).IsAssignableFrom(type)) return IsList;
			else if (typeof(IVariableDictionary).IsAssignableFrom(type)) return IsDictionary;
			else return _reference is T;
		}

		public T As<T>()
		{
			T value;
			TryGet(out value);
			return value;
		}

		public bool TryGet<T>(out T value)
		{
			if (Type == VariableType.Empty)
			{
				value = default;
				return false;
			}
			else
			{
				value = (T)Box();
				return true;
			}
		}

		#endregion

		#region Collection

		public bool IsCollection => Type == VariableType.Object || Type == VariableType.Dictionary;
		public ObjectVariableCollection AsCollection => new ObjectVariableCollection(_reference);
		public bool TryGetCollection(out ObjectVariableCollection collection) { collection = AsCollection; return collection.IsValid; }

		public bool IsArray => _reference is IVariableArray;
		public IVariableArray AsArray => _reference as IVariableArray;
		public bool TryGetArray(out IVariableArray array) { array = AsArray; return array != null; }

		#endregion

		#region Empty

		public static readonly Variable Empty = CreateValue(VariableType.Empty, new ValueData());
		public bool IsEmpty => Type == VariableType.Empty;

		#endregion

		#region Bool

		public static Variable Bool(bool value) => CreateValue(VariableType.Bool, new ValueData { Bool = value });
		public bool IsBool => Type == VariableType.Bool;
		public bool AsBool => _value.Bool;
		public bool TryGetBool(out bool value) { value = AsBool; return IsBool; }

		#endregion

		#region Int

		public static Variable Int(int value) => CreateValue(VariableType.Int, new ValueData { Int = value });
		public bool IsInt => Type == VariableType.Int;
		public int AsInt => _value.Int;
		public bool TryGetInt(out int value) { value = AsInt; return IsInt; }

		#endregion

		#region Float

		public static Variable Float(float value) => CreateValue(VariableType.Float, new ValueData { Float = value });
		public bool IsFloat => Type == VariableType.Float || IsInt;
		public float AsFloat => IsInt ? AsInt : _value.Float;
		public bool TryGetFloat(out float value) { value = AsFloat; return IsFloat; }

		#endregion

		#region Vector2Int

		public static Variable Vector2Int(Vector2Int value) => CreateValue(VariableType.Vector2Int, new ValueData { Vector2Int = value });
		public bool IsVector2Int => Type == VariableType.Vector2Int;
		public Vector2Int AsVector2Int => _value.Vector2Int;
		public bool TryGetVector2Int(out Vector2Int value) { value = AsVector2Int; return IsVector2Int; }

		#endregion

		#region Vector3Int

		public static Variable Vector3Int(Vector3Int value) => CreateValue(VariableType.Vector3Int, new ValueData { Vector3Int = value });
		public bool IsVector3Int => Type == VariableType.Vector3Int || IsVector2Int;
		public Vector3Int AsVector3Int => Type == VariableType.Vector2Int ? (Vector3Int)AsVector2Int : _value.Vector3Int;
		public bool TryGetVector3Int(out Vector3Int value) { value = AsVector3Int; return IsVector3Int; }

		#endregion

		#region RectInt

		public static Variable RectInt(RectInt value) => CreateValue(VariableType.RectInt, new ValueData { RectInt = value });
		public bool IsRectInt => Type == VariableType.RectInt;
		public RectInt AsRectInt => _value.RectInt;
		public bool TryGetRectInt(out RectInt value) { value = AsRectInt; return IsRectInt; }

		#endregion

		#region BoundsInt

		public static Variable BoundsInt(BoundsInt value) => CreateValue(VariableType.BoundsInt, new ValueData { BoundsInt = value });
		public bool IsBoundsInt => Type == VariableType.BoundsInt;
		public BoundsInt AsBoundsInt => _value.BoundsInt;
		public bool TryGetBoundsInt(out BoundsInt value) { value = AsBoundsInt; return IsBoundsInt; }

		#endregion

		#region Vector2

		public static Variable Vector2(Vector2 value) => CreateValue(VariableType.Vector2, new ValueData { Vector2 = value });
		public bool IsVector2 => Type == VariableType.Vector2 || IsVector2Int;
		public Vector2 AsVector2 => IsVector2Int ? AsVector2Int : _value.Vector2;
		public bool TryGetVector2(out Vector2 value) { value = AsVector2; return IsVector2; }

		#endregion

		#region Vector3

		public static Variable Vector3(Vector3 value) => CreateValue(VariableType.Vector3, new ValueData { Vector3 = value });
		public bool IsVector3 => Type == VariableType.Vector3 || IsVector2 || IsVector3Int;
		public Vector3 AsVector3 => IsVector3Int ? AsVector3Int : (IsVector2 ? (Vector3)AsVector2 : _value.Vector3);
		public bool TryGetVector3(out Vector3 value) { value = AsVector3; return IsVector3; }

		#endregion

		#region Vector4

		public static Variable Vector4(Vector4 value) => CreateValue(VariableType.Vector4, new ValueData { Vector4 = value });
		public bool IsVector4 => Type == VariableType.Vector4 || IsVector3;
		public Vector4 AsVector4 => IsVector3 ? (Vector4)AsVector3 : _value.Vector4;
		public bool TryGetVector4(out Vector4 value) { value = AsVector4; return IsVector4; }

		#endregion

		#region Quaternion

		public static Variable Quaternion(Quaternion value) => CreateValue(VariableType.Quaternion, new ValueData { Quaternion = value });
		public bool IsQuaternion => Type == VariableType.Quaternion;
		public Quaternion AsQuaternion => _value.Quaternion;
		public bool TryGetQuaternion(out Quaternion value) { value = AsQuaternion; return IsQuaternion; }

		#endregion

		#region Rect

		public static Variable Rect(Rect value) => CreateValue(VariableType.Rect, new ValueData { Rect = value });
		public bool IsRect => Type == VariableType.Rect || IsRectInt;
		public Rect AsRect => IsRectInt ? new Rect(AsRectInt.position, AsRectInt.size) : _value.Rect;
		public bool TryGetRect(out Rect value) { value = AsRect; return IsRect; }

		#endregion

		#region Bounds

		public static Variable Bounds(Bounds value) => CreateValue(VariableType.Bounds, new ValueData { Bounds = value });
		public bool IsBounds => Type == VariableType.Bounds || IsBoundsInt;
		public Bounds AsBounds => IsBoundsInt ? new Bounds(AsBoundsInt.center, AsBoundsInt.size) : _value.Bounds;
		public bool TryGetBounds(out Bounds value) { value = AsBounds; return IsBounds; }

		#endregion

		#region Color

		public static Variable Color(Color value) => CreateValue(VariableType.Color, new ValueData { Color = value });
		public bool IsColor => Type == VariableType.Color;
		public Color AsColor => _value.Color;
		public bool TryGetColor(out Color value) { value = AsColor; return IsColor; }

		#endregion

		#region Enum

		public enum InvalidEnum
		{
			Invalid
		}

		// without using Reflection Emit there is no way to cast a generic Enum to/from int without boxing so enums
		// are stored as reference types until Emit becomes available in Unity for all platforms

		public static bool IsValidEnumType(Type type) => type != null && type.IsEnum && System.Enum.GetValues(type).Length > 0;

		public static Variable Enum(Enum value) => CreateReference(VariableType.Enum, value ?? InvalidEnum.Invalid);
		public bool IsEnum => Type == VariableType.Enum;
		public Enum AsEnum => _reference as Enum;
		public bool TryGetEnum(out Enum value) { value = AsEnum; return value != null; }

		public static Variable Enum<EnumType>(EnumType value) where EnumType : struct, Enum => CreateReference(VariableType.Enum, value);
		public bool HasEnum<EnumType>() where EnumType : struct, Enum => GetAsEnum(typeof(EnumType)) != null;
		public EnumType GetEnum<EnumType>() where EnumType : struct, Enum { var e = GetAsEnum(typeof(EnumType)); return e != null ? (EnumType)e : default; }
		public bool TryGetEnum<EnumType>(out EnumType value) where EnumType : struct, Enum { var e = GetAsEnum(typeof(EnumType)); value = e != null ? (EnumType)e : default; return e != null; }

		public bool HasEnum(Type enumType) => GetAsEnum(enumType) != null;
		public Enum GetEnum(Type enumType) => GetAsEnum(enumType);
		public bool TryGetEnum(Type enumType, out Enum value) { value = GetAsEnum(enumType); return value != null; }
		public Type EnumType => IsEnum ? _reference.GetType() : null;

		private Enum GetAsEnum(Type type)
		{
			if (Type == VariableType.Enum)
			{
				return _reference as Enum;
			}
			else if (IsString)
			{
				try { return System.Enum.Parse(type, AsString) as Enum; }
				catch { }
			}

			return null;
		}

		#endregion

		#region String

		public static Variable String(string value) => CreateReference(VariableType.String, value ?? string.Empty);
		public bool IsString => Type == VariableType.String;
		public string AsString => _reference as string;
		public bool TryGetString(out string value) { value = AsString; return value != null; }

		#endregion

		#region List

		public static Variable List(IVariableList value) => CreateReference(VariableType.List, value ?? new VariableList());
		public bool IsList => _reference is IVariableList;
		public IVariableList AsList => _reference as IVariableList;
		public bool TryGetList(out IVariableList value) { value = AsList; return value != null; }

		#endregion

		#region Dictionary

		public static Variable Dictionary(IVariableDictionary value) => CreateReference(VariableType.Dictionary, value ?? new VariableDictionary());
		public bool IsDictionary => _reference is IVariableDictionary;
		public IVariableDictionary AsDictionary => _reference as IVariableDictionary;
		public bool TryGetDictionary(out IVariableDictionary value) { value = AsDictionary; return value != null; }

		#endregion

		#region Object

		public static Variable Object(object value) => CreateObject(value);
		public bool IsObject => Type == VariableType.Object;
		public object AsObject => IsObject ? _reference : null;
		public bool TryGetObject(out object value) { value = AsObject; return IsObject; }
		public bool IsNullObject => Type == VariableType.Object && _reference == null;

		public bool HasObject<ObjectType>() => IsObject && _reference is ObjectType;
		public ObjectType GetObject<ObjectType>() => IsObject && _reference is ObjectType obj ? obj : default;
		public bool TryGetObject<ObjectType>(out ObjectType value) { value = GetObject<ObjectType>(); return value != null; }

		public bool HasObject(Type objectType) => IsObject ? ObjectType.IsAssignableFrom(_reference.GetType()) : false;
		public Object GetObject(Type objectType) => HasObject(objectType) ? _reference as Object : null;
		public bool TryGetObject(Type objectType, out Object value) { value = GetObject(objectType); return value != null; }
		public Type ObjectType => IsObject ? (_reference != null ? _reference.GetType() : typeof(object)) : null;

		private static Variable CreateObject(object reference)
		{
			// make sure fake null unity objects are stored as real null

			try
			{
				// during loading (or any background thread) the fake null check will throw for fake nulls

				if (reference is Object obj && obj == null)
					reference = null;
			}
			catch
			{
				reference = null;
			}

			return CreateReference(VariableType.Object, reference);
		}

		#endregion
	}
}