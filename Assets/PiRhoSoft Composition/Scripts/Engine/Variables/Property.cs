using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class Property
	{
		public string Name;

		#region Creation

		public static Property Create(Type ownerType, FieldInfo info, bool allowRead, bool allowWrite)
		{
			var property = Create(ownerType);

			property.Name = info.Name;

			if (info.FieldType == typeof(bool)) property.SetupAsBool(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(int)) property.SetupAsInt(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(float)) property.SetupAsFloat(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Vector2Int)) property.SetupAsInt2(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Vector3Int)) property.SetupAsInt3(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(RectInt)) property.SetupAsIntRect(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(BoundsInt)) property.SetupAsIntBounds(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Vector2)) property.SetupAsVector2(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Vector3)) property.SetupAsVector3(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Vector4)) property.SetupAsVector4(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Quaternion)) property.SetupAsQuaternion(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Rect)) property.SetupAsRect(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Bounds)) property.SetupAsBounds(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(Color)) property.SetupAsColor(info, allowRead, allowWrite);
			else if (info.FieldType == typeof(string)) property.SetupAsString(info, allowRead, allowWrite);
			else if (info.FieldType.IsEnum) property.SetupAsEnum(info, allowRead, allowWrite);
			else if (typeof(Object).IsAssignableFrom(info.FieldType)) property.SetupAsObject(info, allowRead, allowWrite);
			else if (typeof(IVariableStore).IsAssignableFrom(info.FieldType)) property.SetupAsStore(info, allowRead, allowWrite);
			else if (IsSupportedList(info.FieldType)) property.SetupAsList(info, allowRead, allowWrite);
			else return null;

			return property;
		}

		public static Property Create(Type ownerType, PropertyInfo info, bool allowRead, bool allowWrite)
		{
			var property = Create(ownerType);

			property.Name = info.Name;

			var getMethod = allowRead ? info.GetGetMethod(true) : null;
			var setMethod = allowWrite ? info.GetSetMethod(true) : null;

			if (info.PropertyType == typeof(bool)) property.SetupAsBool(getMethod, setMethod);
			else if (info.PropertyType == typeof(int)) property.SetupAsInt(getMethod, setMethod);
			else if (info.PropertyType == typeof(float)) property.SetupAsFloat(getMethod, setMethod);
			else if (info.PropertyType == typeof(Vector2Int)) property.SetupAsInt2(getMethod, setMethod);
			else if (info.PropertyType == typeof(Vector3Int)) property.SetupAsInt3(getMethod, setMethod);
			else if (info.PropertyType == typeof(RectInt)) property.SetupAsIntRect(getMethod, setMethod);
			else if (info.PropertyType == typeof(BoundsInt)) property.SetupAsIntBounds(getMethod, setMethod);
			else if (info.PropertyType == typeof(Vector2)) property.SetupAsVector2(getMethod, setMethod);
			else if (info.PropertyType == typeof(Vector3)) property.SetupAsVector3(getMethod, setMethod);
			else if (info.PropertyType == typeof(Vector4)) property.SetupAsVector4(getMethod, setMethod);
			else if (info.PropertyType == typeof(Quaternion)) property.SetupAsQuaternion(getMethod, setMethod);
			else if (info.PropertyType == typeof(Rect)) property.SetupAsRect(getMethod, setMethod);
			else if (info.PropertyType == typeof(Bounds)) property.SetupAsBounds(getMethod, setMethod);
			else if (info.PropertyType == typeof(Color)) property.SetupAsColor(getMethod, setMethod);
			else if (info.PropertyType == typeof(string)) property.SetupAsString(getMethod, setMethod);
			else if (info.PropertyType.IsEnum) property.SetupAsEnum(info.PropertyType, getMethod, setMethod);
			else if (typeof(Object).IsAssignableFrom(info.PropertyType)) property.SetupAsObject(info.PropertyType, getMethod, setMethod);
			else if (typeof(IVariableStore).IsAssignableFrom(info.PropertyType)) property.SetupAsStore(info.PropertyType, getMethod, setMethod);
			else if (IsSupportedList(info.PropertyType)) property.SetupAsList(info.PropertyType, getMethod, setMethod);
			else return null;

			return property;
		}

		private static Property Create(Type ownerType)
		{
			var open = typeof(Property<>);
			var closed = open.MakeGenericType(ownerType);

			return Activator.CreateInstance(closed) as Property;
		}

		#endregion

		#region Setup

		protected abstract void SetupAsBool(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsInt(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsFloat(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsInt2(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsInt3(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsIntRect(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsIntBounds(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsVector2(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsVector3(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsVector4(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsQuaternion(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsRect(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsBounds(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsColor(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsString(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsEnum(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsObject(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsStore(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsList(FieldInfo field, bool allowRead, bool allowWrite);

		protected abstract void SetupAsBool(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsInt(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsFloat(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsInt2(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsInt3(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsIntRect(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsIntBounds(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsVector2(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsVector3(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsVector4(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsQuaternion(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsRect(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsBounds(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsColor(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsString(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsEnum(Type enumType, MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsObject(Type objectType, MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsStore(Type storeType, MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsList(Type listType, MethodInfo getMethod, MethodInfo setMethod);

		#endregion

		#region Access

		public abstract VariableValue Get(object obj);
		public abstract SetVariableResult Set(object obj, VariableValue value);

		#endregion

		#region Support Testing

		public static bool IsSupportedType(Type type)
		{
			return type == typeof(bool)
				|| type == typeof(int)
				|| type == typeof(float)
				|| type == typeof(Vector2Int)
				|| type == typeof(Vector3Int)
				|| type == typeof(RectInt)
				|| type == typeof(BoundsInt)
				|| type == typeof(Vector2)
				|| type == typeof(Vector3)
				|| type == typeof(Vector4)
				|| type == typeof(Quaternion)
				|| type == typeof(Rect)
				|| type == typeof(Bounds)
				|| type == typeof(Color)
				|| type == typeof(string)
				|| type.IsEnum
				|| typeof(Object).IsAssignableFrom(type)
				|| typeof(IVariableStore).IsAssignableFrom(type);
		}

		public static bool IsSupportedList(Type type)
		{
			var interfaces = type.GetInterfaces();

			foreach (var i in interfaces)
			{
				if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) && IsSupportedType(i.GetGenericArguments()[0]))
					return true;
			}

			return false;
		}

		#endregion
	}

	public class Property<OwnerType> : Property
	{
		public Func<OwnerType, VariableValue> Getter;
		public Func<OwnerType, VariableValue, SetVariableResult> Setter;

		#region Field Setup

		protected override void SetupAsBool(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<bool>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetBool(obj, value, field);
		}

		protected override void SetupAsInt(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt(obj, value, field);
		}

		protected override void SetupAsFloat(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<float>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetFloat(obj, value, field);
		}

		protected override void SetupAsInt2(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Vector2Int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt2(obj, value, field);
		}

		protected override void SetupAsInt3(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Vector3Int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt3(obj, value, field);
		}

		protected override void SetupAsIntRect(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<RectInt>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetIntRect(obj, value, field);
		}

		protected override void SetupAsIntBounds(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<BoundsInt>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetIntBounds(obj, value, field);
		}

		protected override void SetupAsVector2(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Vector2>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector2(obj, value, field);
		}

		protected override void SetupAsVector3(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Vector3>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector3(obj, value, field);
		}

		protected override void SetupAsVector4(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Vector4>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector4(obj, value, field);
		}

		protected override void SetupAsQuaternion(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Quaternion>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetQuaternion(obj, value, field);
		}

		protected override void SetupAsRect(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Rect>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetRect(obj, value, field);
		}

		protected override void SetupAsBounds(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Bounds>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetBounds(obj, value, field);
		}

		protected override void SetupAsColor(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetValue<Color>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetColor(obj, value, field);
		}

		protected override void SetupAsString(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetString(obj, field);
			if (allowWrite) Setter = (obj, value) => SetString(obj, value, field);
		}

		protected override void SetupAsEnum(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetEnum(obj, field);
			if (allowWrite) Setter = (obj, value) => SetEnum(obj, value, field);
		}

		protected override void SetupAsObject(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetReference(obj, field);
			if (allowWrite) Setter = (obj, value) => SetReference(obj, value, field);
		}

		protected override void SetupAsStore(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetReference(obj, field);
			if (allowWrite) Setter = (obj, value) => SetReference(obj, value, field);
		}

		protected override void SetupAsList(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => GetList(obj, field);
			// lists are always read only
		}

		#endregion

		#region Field Getters

		private static VariableValue GetValue<T>(OwnerType owner, FieldInfo field)
		{
			var value = (T)field.GetValue(owner);
			return VariableValue.CreateValue(value);
		}

		private static VariableValue GetEnum(OwnerType owner, FieldInfo field)
		{
			var value = (Enum)field.GetValue(owner);
			return VariableValue.Create(value);
		}

		private static VariableValue GetString(OwnerType owner, FieldInfo field)
		{
			var value = (string)field.GetValue(owner);
			return VariableValue.Create(value);
		}

		private static VariableValue GetReference(OwnerType owner, FieldInfo field)
		{
			var value = field.GetValue(owner);
			return VariableValue.CreateReference(value);
		}

		private static VariableValue GetList(OwnerType owner, FieldInfo field)
		{
			var list = field.GetValue(owner);
			var adapter = ListAdapter.Create(list);
			return VariableValue.Create(adapter);
		}

		#endregion

		#region Field Setters

		// as of now Unity/IL2CPP does not support Expression compilation for value types so using reflection at
		// runtime is the only option for fields (Reflection.Emit is also not available)
		// more info here: https://forum.unity.com/threads/are-c-expression-trees-or-ilgenerator-allowed-on-ios.489498/

		private static SetVariableResult SetBool(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Bool)
			{
				field.SetValue(owner, value.Bool);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Int)
			{
				field.SetValue(owner, value.Int);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetFloat(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.HasNumber)
			{
				field.SetValue(owner, value.Number);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt2(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Int2)
			{
				field.SetValue(owner, value.Int2);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt3(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Int3)
			{
				field.SetValue(owner, value.Int3);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetIntRect(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.IntRect)
			{
				field.SetValue(owner, value.IntRect);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetIntBounds(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.IntBounds)
			{
				field.SetValue(owner, value.IntBounds);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector2(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Vector2)
			{
				field.SetValue(owner, value.Vector2);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector3(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Vector3)
			{
				field.SetValue(owner, value.Vector3);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector4(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Vector4)
			{
				field.SetValue(owner, value.Vector4);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetQuaternion(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Quaternion)
			{
				field.SetValue(owner, value.Quaternion);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetRect(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Rect)
			{
				field.SetValue(owner, value.Rect);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetBounds(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Bounds)
			{
				field.SetValue(owner, value.Bounds);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetColor(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Color)
			{
				field.SetValue(owner, value.Color);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetString(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.String)
			{
				field.SetValue(owner, value.String);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetEnum(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.HasEnumType(field.FieldType))
			{
				field.SetValue(owner, value.Enum);
				return SetVariableResult.Success;
			}
			else if (value.HasString)
			{
				try
				{
					var e = (Enum)Enum.Parse(field.FieldType, value.String);
					field.SetValue(owner, e);
					return SetVariableResult.Success;
				}
				catch { }
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetReference(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.HasReference)
			{
				if (value.Reference == null)
				{
					field.SetValue(owner, null);
					return SetVariableResult.Success;
				}
				else if (field.FieldType.IsAssignableFrom(value.ReferenceType))
				{
					field.SetValue(owner, value.Reference);
					return SetVariableResult.Success;
				}
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

		#region Property Setup

		protected override void SetupAsBool(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, bool>)getMethod?.CreateDelegate(typeof(Func<OwnerType, bool>));
			var set = (Action<OwnerType, bool>)setMethod?.CreateDelegate(typeof(Action<OwnerType, bool>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetBool(obj, value, set);
		}

		protected override void SetupAsInt(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, int>));
			var set = (Action<OwnerType, int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, int>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetInt(obj, value, set);
		}

		protected override void SetupAsFloat(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, float>)getMethod?.CreateDelegate(typeof(Func<OwnerType, float>));
			var set = (Action<OwnerType, float>)setMethod?.CreateDelegate(typeof(Action<OwnerType, float>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetFloat(obj, value, set);
		}

		protected override void SetupAsInt2(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector2Int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector2Int>));
			var set = (Action<OwnerType, Vector2Int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector2Int>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetInt2(obj, value, set);
		}

		protected override void SetupAsInt3(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector3Int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector3Int>));
			var set = (Action<OwnerType, Vector3Int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector3Int>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetInt3(obj, value, set);
		}

		protected override void SetupAsIntRect(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, RectInt>)getMethod?.CreateDelegate(typeof(Func<OwnerType, RectInt>));
			var set = (Action<OwnerType, RectInt>)setMethod?.CreateDelegate(typeof(Action<OwnerType, RectInt>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetIntRect(obj, value, set);
		}

		protected override void SetupAsIntBounds(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, BoundsInt>)getMethod?.CreateDelegate(typeof(Func<OwnerType, BoundsInt>));
			var set = (Action<OwnerType, BoundsInt>)setMethod?.CreateDelegate(typeof(Action<OwnerType, BoundsInt>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetIntBounds(obj, value, set);
		}

		protected override void SetupAsVector2(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector2>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector2>));
			var set = (Action<OwnerType, Vector2>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector2>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetVector2(obj, value, set);
		}

		protected override void SetupAsVector3(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector3>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector3>));
			var set = (Action<OwnerType, Vector3>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector3>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetVector3(obj, value, set);
		}

		protected override void SetupAsVector4(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector4>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector4>));
			var set = (Action<OwnerType, Vector4>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector4>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetVector4(obj, value, set);
		}

		protected override void SetupAsQuaternion(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Quaternion>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Quaternion>));
			var set = (Action<OwnerType, Quaternion>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Quaternion>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetQuaternion(obj, value, set);
		}

		protected override void SetupAsRect(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Rect>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Rect>));
			var set = (Action<OwnerType, Rect>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Rect>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetRect(obj, value, set);
		}

		protected override void SetupAsBounds(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Bounds>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Bounds>));
			var set = (Action<OwnerType, Bounds>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Bounds>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetBounds(obj, value, set);
		}

		protected override void SetupAsColor(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Color>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Color>));
			var set = (Action<OwnerType, Color>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Color>));

			if (get != null) Getter = obj => GetValue(obj, get);
			if (set != null) Setter = (obj, value) => SetColor(obj, value, set);
		}

		protected override void SetupAsString(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, string>)getMethod?.CreateDelegate(typeof(Func<OwnerType, string>));
			var set = (Action<OwnerType, string>)setMethod?.CreateDelegate(typeof(Action<OwnerType, string>));

			if (get != null) Getter = obj => GetString(obj, get);
			if (set != null) Setter = (obj, value) => SetString(obj, value, set);
		}

		protected override void SetupAsEnum(Type enumType, MethodInfo getMethod, MethodInfo setMethod)
		{
			var creator = DelegateCreator.Create(enumType);

			var get = getMethod != null ? creator.CreateGetter(getMethod) : null;
			var set = setMethod != null ? creator.CreateSetter(setMethod) : null;

			if (get != null) Getter = obj => GetEnum(obj, get);
			if (set != null) Setter = (obj, value) => SetEnum(enumType,  obj, value, set);
		}

		protected override void SetupAsObject(Type objectType, MethodInfo getMethod, MethodInfo setMethod)
		{
			var creator = DelegateCreator.Create(objectType);

			var get = getMethod != null ? creator.CreateGetter(getMethod) : null;
			var set = setMethod != null ? creator.CreateSetter(setMethod) : null;

			if (get != null) Getter = obj => GetReference(obj, get);
			if (set != null) Setter = (obj, value) => SetReference(objectType, obj, value, set);
		}

		protected override void SetupAsStore(Type storeType, MethodInfo getMethod, MethodInfo setMethod)
		{
			var creator = DelegateCreator.Create(storeType);

			var get = getMethod != null ? creator.CreateGetter(getMethod) : null;
			var set = setMethod != null ? creator.CreateSetter(setMethod) : null;

			if (get != null) Getter = obj => GetReference(obj, get);
			if (set != null) Setter = (obj, value) => SetReference(storeType, obj, value, set);
		}

		protected override void SetupAsList(Type listType, MethodInfo getMethod, MethodInfo setMethod)
		{
			var creator = DelegateCreator.Create(listType);

			var get = getMethod != null ? creator.CreateGetter(getMethod) : null;

			if (get != null) Getter = obj => GetList(obj, get);
			// lists are always read only
		}

		#region Delegate Casting

		private abstract class DelegateCreator
		{
			public static DelegateCreator Create(Type type)
			{
				var open = typeof(DelegateCreator<>);
				var closed = open.MakeGenericType(type);

				return Activator.CreateInstance(closed) as DelegateCreator;
			}

			public abstract Func<OwnerType, object> CreateGetter(MethodInfo getMethod);
			public abstract Action<OwnerType, object> CreateSetter(MethodInfo setMethod);
		}

		private class DelegateCreator<T> : DelegateCreator
		{
			public override Func<OwnerType, object> CreateGetter(MethodInfo getMethod)
			{
				var getter = (Func<OwnerType, T>)getMethod.CreateDelegate(typeof(Func<OwnerType, T>));
				return (owner) => getter(owner);
			}

			public override Action<OwnerType, object> CreateSetter(MethodInfo setMethod)
			{
				var setter = (Action<OwnerType, T>)setMethod.CreateDelegate(typeof(Action<OwnerType, T>));
				return (owner, value) => setter(owner, (T)value);
			}
		}

		#endregion

		#endregion

		#region Property Getters

		private static VariableValue GetValue<T>(OwnerType owner, Func<OwnerType, T> getter)
		{
			var value = getter(owner);
			return VariableValue.CreateValue(value);
		}

		private static VariableValue GetEnum(OwnerType owner, Func<OwnerType, object> getter)
		{
			var value = (Enum)getter(owner);
			return VariableValue.Create(value);
		}

		private static VariableValue GetString(OwnerType owner, Func<OwnerType, string> getter)
		{
			var value = getter(owner);
			return VariableValue.Create(value);
		}

		private static VariableValue GetReference(OwnerType owner, Func<OwnerType, object> getter)
		{
			var value = getter(owner);
			return VariableValue.CreateReference(value);
		}

		private static VariableValue GetList(OwnerType owner, Func<OwnerType, object> getter)
		{
			var list = getter(owner);
			var adapter = ListAdapter.Create(list);
			return VariableValue.Create(adapter);
		}

		#endregion

		#region Property Setters

		private static SetVariableResult SetBool(OwnerType owner, VariableValue value, Action<OwnerType, bool> setter)
		{
			if (value.Type == VariableType.Bool)
			{
				setter(owner, value.Bool);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt(OwnerType owner, VariableValue value, Action<OwnerType, int> setter)
		{
			if (value.Type == VariableType.Int)
			{
				setter(owner, value.Int);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetFloat(OwnerType owner, VariableValue value, Action<OwnerType, float> setter)
		{
			if (value.Type == VariableType.Float || value.Type == VariableType.Int)
			{
				setter(owner, value.Number);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt2(OwnerType owner, VariableValue value, Action<OwnerType, Vector2Int> setter)
		{
			if (value.Type == VariableType.Int2)
			{
				setter(owner, value.Int2);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt3(OwnerType owner, VariableValue value, Action<OwnerType, Vector3Int> setter)
		{
			if (value.Type == VariableType.Int3)
			{
				setter(owner, value.Int3);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetIntRect(OwnerType owner, VariableValue value, Action<OwnerType, RectInt> setter)
		{
			if (value.Type == VariableType.IntRect)
			{
				setter(owner, value.IntRect);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetIntBounds(OwnerType owner, VariableValue value, Action<OwnerType, BoundsInt> setter)
		{
			if (value.Type == VariableType.IntBounds)
			{
				setter(owner, value.IntBounds);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector2(OwnerType owner, VariableValue value, Action<OwnerType, Vector2> setter)
		{
			if (value.Type == VariableType.Vector2)
			{
				setter(owner, value.Vector2);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector3(OwnerType owner, VariableValue value, Action<OwnerType, Vector3> setter)
		{
			if (value.Type == VariableType.Vector3)
			{
				setter(owner, value.Vector3);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetVector4(OwnerType owner, VariableValue value, Action<OwnerType, Vector4> setter)
		{
			if (value.Type == VariableType.Vector4)
			{
				setter(owner, value.Vector4);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetQuaternion(OwnerType owner, VariableValue value, Action<OwnerType, Quaternion> setter)
		{
			if (value.Type == VariableType.Quaternion)
			{
				setter(owner, value.Quaternion);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetRect(OwnerType owner, VariableValue value, Action<OwnerType, Rect> setter)
		{
			if (value.Type == VariableType.Rect)
			{
				setter(owner, value.Rect);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetBounds(OwnerType owner, VariableValue value, Action<OwnerType, Bounds> setter)
		{
			if (value.Type == VariableType.Bounds)
			{
				setter(owner, value.Bounds);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetColor(OwnerType owner, VariableValue value, Action<OwnerType, Color> setter)
		{
			if (value.Type == VariableType.Color)
			{
				setter(owner, value.Color);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetString(OwnerType owner, VariableValue value, Action<OwnerType, string> setter)
		{
			if (value.Type == VariableType.String)
			{
				setter(owner, value.String);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetEnum(Type enumType, OwnerType owner, VariableValue value, Action<OwnerType, object> setter)
		{
			if (value.HasEnumType(enumType))
			{
				setter(owner, value.Enum);
				return SetVariableResult.Success;
			}
			else if (value.Type == VariableType.String)
			{
				try
				{
					var e = (Enum)Enum.Parse(enumType, value.String);
					setter(owner, e);
					return SetVariableResult.Success;
				}
				catch { }
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetReference(Type propertyType, OwnerType owner, VariableValue value, Action<OwnerType, object> setter)
		{
			if (value.HasReference)
			{
				if (value.Reference == null)
				{
					setter(owner, null);
					return SetVariableResult.Success;
				}
				else if (propertyType.IsAssignableFrom(value.ReferenceType))
				{
					setter(owner, value.Reference);
					return SetVariableResult.Success;
				}
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

		#region Access

		public override VariableValue Get(object obj)
		{
			if (Getter != null) return Getter((OwnerType)obj);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Set(object obj, VariableValue value)
		{
			if (Setter != null) return Setter((OwnerType)obj, value);
			else return SetVariableResult.ReadOnly;
		}

		#endregion

		#region List Adapters

		private abstract class ListAdapter : IVariableList
		{
			public abstract int Count { get; }

			public static IVariableList Create(object list)
			{
				ListAdapter adapter = null;
				var itemType = list.GetType().GetGenericArguments()[0];

				if (itemType == typeof(bool)) adapter = new BoolListAdapter();
				else if (itemType == typeof(int)) adapter = new IntListAdapter();
				else if (itemType == typeof(float)) adapter = new FloatListAdapter();
				else if (itemType == typeof(Vector2Int)) adapter = new Int2ListAdapter();
				else if (itemType == typeof(Vector3Int)) adapter = new Int3ListAdapter();
				else if (itemType == typeof(RectInt)) adapter = new IntRectListAdapter();
				else if (itemType == typeof(BoundsInt)) adapter = new IntBoundsListAdapter();
				else if (itemType == typeof(Vector2)) adapter = new Vector2ListAdapter();
				else if (itemType == typeof(Vector3)) adapter = new Vector3ListAdapter();
				else if (itemType == typeof(Vector4)) adapter = new Vector4ListAdapter();
				else if (itemType == typeof(Quaternion)) adapter = new QuaternionListAdapter();
				else if (itemType == typeof(RectInt)) adapter = new RectListAdapter();
				else if (itemType == typeof(BoundsInt)) adapter = new BoundsListAdapter();
				else if (itemType == typeof(Color)) adapter = new ColorListAdapter();
				else if (itemType == typeof(string)) adapter = new StringListAdapter();
				else if (itemType.IsEnum) adapter = (ListAdapter)Activator.CreateInstance(typeof(EnumListAdapter<>).MakeGenericType(itemType));
				else adapter = (ListAdapter)Activator.CreateInstance(typeof(ObjectListAdapter<>).MakeGenericType(itemType));

				return adapter;
			}

			protected abstract void Setup(object list, bool allowSet, bool allowAdd, bool allowRemove);

			public abstract VariableValue GetVariable(int index);
			public abstract SetVariableResult SetVariable(int index, VariableValue value);
			public abstract SetVariableResult AddVariable(VariableValue value);
			public abstract SetVariableResult RemoveVariable(int index);
		}

		private abstract class ListAdapter<T> : ListAdapter
		{
			protected IList<T> _list;

			private bool _allowSet;
			private bool _allowAdd;
			private bool _allowRemove;

			public override int Count => _list.Count;

			protected override void Setup(object list, bool allowSet, bool allowAdd, bool allowRemove)
			{
				_list = list as IList<T>;
				_allowSet = allowSet;
				_allowAdd = allowAdd;
				_allowRemove = allowRemove;
			}

			public override VariableValue GetVariable(int index)
			{
				if (index >= 0 && index <= _list.Count)
					return Get(index);
				else
					return VariableValue.Empty;
			}

			public override SetVariableResult SetVariable(int index, VariableValue value)
			{
				if (!_allowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (index >= 0 && index <= _list.Count)
				{
					if (Set(index, value))
						return SetVariableResult.Success;
					else
						return SetVariableResult.TypeMismatch;
				}

				return SetVariableResult.NotFound;
			}

			public override SetVariableResult AddVariable(VariableValue value)
			{
				if (!_allowAdd)
					return SetVariableResult.ReadOnly;
				else if (Add(value))
					return SetVariableResult.Success;

				return SetVariableResult.TypeMismatch;
			}

			public override SetVariableResult RemoveVariable(int index)
			{
				if (!_allowRemove)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (index >= 0 && index <= _list.Count)
				{
					_list.RemoveAt(index);
					return SetVariableResult.Success;
				}

				return SetVariableResult.NotFound;
			}

			protected bool Set(int index, T value)
			{
				_list[index] = value;
				return true;
			}

			protected bool Add(T value)
			{
				_list.Add(value);
				return true;
			}

			protected abstract VariableValue Get(int index);
			protected abstract bool Set(int index, VariableValue value);
			protected abstract bool Add(VariableValue value);
		}

		private class BoolListAdapter : ListAdapter<bool>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetBool(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetBool(out var v) && Add(v);
		}

		private class IntListAdapter : ListAdapter<int>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetInt(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetInt(out var v) && Add(v);
		}

		private class FloatListAdapter : ListAdapter<float>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetFloat(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetFloat(out var v) && Add(v);
		}

		private class Int2ListAdapter : ListAdapter<Vector2Int>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetInt2(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetInt2(out var v) && Add(v);
		}

		private class Int3ListAdapter : ListAdapter<Vector3Int>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetInt3(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetInt3(out var v) && Add(v);
		}

		private class IntRectListAdapter : ListAdapter<RectInt>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetIntRect(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetIntRect(out var v) && Add(v);
		}

		private class IntBoundsListAdapter : ListAdapter<BoundsInt>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetIntBounds(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetIntBounds(out var v) && Add(v);
		}

		private class Vector2ListAdapter : ListAdapter<Vector2>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetVector2(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetVector2(out var v) && Add(v);
		}

		private class Vector3ListAdapter : ListAdapter<Vector3>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetVector3(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetVector3(out var v) && Add(v);
		}

		private class Vector4ListAdapter : ListAdapter<Vector4>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetVector4(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetVector4(out var v) && Add(v);
		}

		private class QuaternionListAdapter : ListAdapter<Quaternion>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetQuaternion(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetQuaternion(out var v) && Add(v);
		}

		private class RectListAdapter : ListAdapter<Rect>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetRect(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetRect(out var v) && Add(v);
		}

		private class BoundsListAdapter : ListAdapter<Bounds>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetBounds(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetBounds(out var v) && Add(v);
		}

		private class ColorListAdapter : ListAdapter<Color>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetColor(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetColor(out var v) && Add(v);
		}

		private class StringListAdapter : ListAdapter<string>
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetString(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetString(out var v) && Add(v);
		}

		private class EnumListAdapter<EnumType> : ListAdapter<EnumType> where EnumType : Enum
		{
			protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetEnum<EnumType>(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetEnum<EnumType>(out var v) && Add(v);
		}

		private class ObjectListAdapter<ObjectType> : ListAdapter<ObjectType> where ObjectType : class
		{
			protected override VariableValue Get(int index) => VariableValue.CreateReference(_list[index]);
			protected override bool Set(int index, VariableValue value) => value.TryGetReference<ObjectType>(out var v) && Set(index, v);
			protected override bool Add(VariableValue value) => value.TryGetReference<ObjectType>(out var v) && Add(v);
		}

		#endregion
	}
}
