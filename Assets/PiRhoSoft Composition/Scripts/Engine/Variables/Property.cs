using System;
using System.Reflection;
using UnityEngine;

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
			else property.SetupAsObject(info, allowRead, allowWrite);

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
			else property.SetupAsObject(info.PropertyType, getMethod, setMethod);

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
		protected abstract void SetupAsObject(FieldInfo field, bool allowRead, bool allowWrite);

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
		protected abstract void SetupAsObject(Type objectType, MethodInfo getMethod, MethodInfo setMethod);

		#endregion

		#region Access

		public abstract VariableValue Get(object obj);
		public abstract SetVariableResult Set(object obj, VariableValue value);

		#endregion
	}

	public class Property<OwnerType> : Property
	{
		public Func<OwnerType, VariableValue> Getter;
		public Func<OwnerType, VariableValue, SetVariableResult> Setter;

		#region Field Setup

		protected override void SetupAsBool(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<bool>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetBool(obj, value, field);
		}

		protected override void SetupAsInt(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt(obj, value, field);
		}

		protected override void SetupAsFloat(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<float>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetFloat(obj, value, field);
		}

		protected override void SetupAsInt2(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Vector2Int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt2(obj, value, field);
		}

		protected override void SetupAsInt3(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Vector3Int>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetInt3(obj, value, field);
		}

		protected override void SetupAsIntRect(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<RectInt>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetIntRect(obj, value, field);
		}

		protected override void SetupAsIntBounds(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<BoundsInt>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetIntBounds(obj, value, field);
		}

		protected override void SetupAsVector2(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Vector2>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector2(obj, value, field);
		}

		protected override void SetupAsVector3(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Vector3>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector3(obj, value, field);
		}

		protected override void SetupAsVector4(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Vector4>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetVector4(obj, value, field);
		}

		protected override void SetupAsQuaternion(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Quaternion>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetQuaternion(obj, value, field);
		}

		protected override void SetupAsRect(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Rect>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetRect(obj, value, field);
		}

		protected override void SetupAsBounds(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Bounds>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetBounds(obj, value, field);
		}

		protected override void SetupAsColor(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<Color>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetColor(obj, value, field);
		}

		protected override void SetupAsString(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<string>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetString(obj, value, field);
		}

		protected override void SetupAsObject(FieldInfo field, bool allowRead, bool allowWrite)
		{
			if (allowRead) Getter = obj => Get<object>(obj, field);
			if (allowWrite) Setter = (obj, value) => SetObject(obj, value, field);
		}

		#endregion

		#region Property Setup

		protected override void SetupAsBool(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, bool>)getMethod?.CreateDelegate(typeof(Func<OwnerType, bool>));
			var set = (Action<OwnerType, bool>)setMethod?.CreateDelegate(typeof(Action<OwnerType, bool>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetBool(obj, value, set);
		}

		protected override void SetupAsInt(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, int>));
			var set = (Action<OwnerType, int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, int>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetInt(obj, value, set);
		}

		protected override void SetupAsFloat(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, float>)getMethod?.CreateDelegate(typeof(Func<OwnerType, float>));
			var set = (Action<OwnerType, float>)setMethod?.CreateDelegate(typeof(Action<OwnerType, float>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetFloat(obj, value, set);
		}

		protected override void SetupAsInt2(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector2Int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector2Int>));
			var set = (Action<OwnerType, Vector2Int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector2Int>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetInt2(obj, value, set);
		}

		protected override void SetupAsInt3(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector3Int>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector3Int>));
			var set = (Action<OwnerType, Vector3Int>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector3Int>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetInt3(obj, value, set);
		}

		protected override void SetupAsIntRect(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, RectInt>)getMethod?.CreateDelegate(typeof(Func<OwnerType, RectInt>));
			var set = (Action<OwnerType, RectInt>)setMethod?.CreateDelegate(typeof(Action<OwnerType, RectInt>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetIntRect(obj, value, set);
		}

		protected override void SetupAsIntBounds(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, BoundsInt>)getMethod?.CreateDelegate(typeof(Func<OwnerType, BoundsInt>));
			var set = (Action<OwnerType, BoundsInt>)setMethod?.CreateDelegate(typeof(Action<OwnerType, BoundsInt>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetIntBounds(obj, value, set);
		}

		protected override void SetupAsVector2(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector2>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector2>));
			var set = (Action<OwnerType, Vector2>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector2>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetVector2(obj, value, set);
		}

		protected override void SetupAsVector3(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector3>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector3>));
			var set = (Action<OwnerType, Vector3>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector3>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetVector3(obj, value, set);
		}

		protected override void SetupAsVector4(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Vector4>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Vector4>));
			var set = (Action<OwnerType, Vector4>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Vector4>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetVector4(obj, value, set);
		}

		protected override void SetupAsQuaternion(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Quaternion>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Quaternion>));
			var set = (Action<OwnerType, Quaternion>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Quaternion>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetQuaternion(obj, value, set);
		}

		protected override void SetupAsRect(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Rect>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Rect>));
			var set = (Action<OwnerType, Rect>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Rect>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetRect(obj, value, set);
		}

		protected override void SetupAsBounds(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Bounds>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Bounds>));
			var set = (Action<OwnerType, Bounds>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Bounds>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetBounds(obj, value, set);
		}

		protected override void SetupAsColor(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, Color>)getMethod?.CreateDelegate(typeof(Func<OwnerType, Color>));
			var set = (Action<OwnerType, Color>)setMethod?.CreateDelegate(typeof(Action<OwnerType, Color>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetColor(obj, value, set);
		}

		protected override void SetupAsString(MethodInfo getMethod, MethodInfo setMethod)
		{
			var get = (Func<OwnerType, string>)getMethod?.CreateDelegate(typeof(Func<OwnerType, string>));
			var set = (Action<OwnerType, string>)setMethod?.CreateDelegate(typeof(Action<OwnerType, string>));

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetString(obj, value, set);
		}

		protected override void SetupAsObject(Type objectType, MethodInfo getMethod, MethodInfo setMethod)
		{
			var creator = DelegateCreator.Create(objectType);

			var get = getMethod != null ? creator.CreateGetter(getMethod) : null;
			var set = setMethod != null ? creator.CreateSetter(setMethod) : null;

			if (get != null) Getter = obj => Get(obj, get);
			if (set != null) Setter = (obj, value) => SetObject(objectType, obj, value, set);
		}

		#endregion

		#region Getters

		private static VariableValue Get<T>(OwnerType owner, Func<OwnerType, T> getter)
		{
			T value = getter(owner);
			return VariableValue.CreateValue(value);
		}

		private static VariableValue Get<T>(OwnerType owner, FieldInfo field)
		{
			T value = (T)field.GetValue(owner);
			return VariableValue.CreateValue(value);
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

		private static SetVariableResult SetObject(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.HasReference && value.Reference != null)
			{
				if (field.FieldType.IsAssignableFrom(value.Reference.GetType()))
				{
					field.SetValue(owner, value.Reference);
					return SetVariableResult.Success;
				}
			}

			return SetVariableResult.TypeMismatch;
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

		private static SetVariableResult SetObject(Type propertyType, OwnerType owner, VariableValue value, Action<OwnerType, object> setter)
		{
			if (value.HasReference && value.Reference != null)
			{
				if (propertyType.IsAssignableFrom(value.Reference.GetType()))
				{
					setter(owner, value.Reference);
					return SetVariableResult.Success;
				}
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

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
	}
}
