using System;
using System.Reflection;

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
		protected abstract void SetupAsString(FieldInfo field, bool allowRead, bool allowWrite);
		protected abstract void SetupAsObject(FieldInfo field, bool allowRead, bool allowWrite);

		protected abstract void SetupAsBool(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsInt(MethodInfo getMethod, MethodInfo setMethod);
		protected abstract void SetupAsFloat(MethodInfo getMethod, MethodInfo setMethod);
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
			return VariableValue.Create(getter(owner));
		}

		private static VariableValue Get<T>(OwnerType owner, FieldInfo field)
		{
			return VariableValue.Create((T)field.GetValue(owner));
		}

		#endregion

		#region Field Setters

		// as of now Unity/IL2CPP does not support Expression compilation for value types so using reflection at
		// runtime is the only option for fields (Reflection.Emit is also not available)
		// more info here: https://forum.unity.com/threads/are-c-expression-trees-or-ilgenerator-allowed-on-ios.489498/

		private static SetVariableResult SetBool(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Boolean)
			{
				field.SetValue(owner, value.Boolean);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Integer)
			{
				field.SetValue(owner, value.Integer);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetFloat(OwnerType owner, VariableValue value, FieldInfo field)
		{
			if (value.Type == VariableType.Number || value.Type == VariableType.Integer)
			{
				field.SetValue(owner, value.Number);
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
			if (value.Type == VariableType.Raw || value.Type == VariableType.Store || value.Type == VariableType.Object)
			{
				if (field.FieldType.IsAssignableFrom(value.RawObject.GetType()))
				{
					field.SetValue(owner, value.RawObject);
					return SetVariableResult.Success;
				}
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

		#region Property Setters

		private static SetVariableResult SetBool(OwnerType owner, VariableValue value, Action<OwnerType, bool> setter)
		{
			if (value.Type == VariableType.Boolean)
			{
				setter(owner, value.Boolean);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetInt(OwnerType owner, VariableValue value, Action<OwnerType, int> setter)
		{
			if (value.Type == VariableType.Integer)
			{
				setter(owner, value.Integer);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		private static SetVariableResult SetFloat(OwnerType owner, VariableValue value, Action<OwnerType, float> setter)
		{
			if (value.Type == VariableType.Number || value.Type == VariableType.Integer)
			{
				setter(owner, value.Number);
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
			if (value.Type == VariableType.Raw || value.Type == VariableType.Store || value.Type == VariableType.Object)
			{
				if (propertyType.IsAssignableFrom(value.RawObject.GetType()))
				{
					setter(owner, value.RawObject);
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
