using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
{
	internal class Property
	{
		// This class uses reflection at runtime to get and set values. For properties, speed and allocation
		// improvements could be made by creating delegates on load (i.e MethodInfo.CreateDelegate) but that
		// would require the necessary intermediate generic class (and generic Action class) to be registered
		// (as described here: https://docs.unity3d.com/Manual/ScriptingRestrictions.html) for AOT platforms.

		// Another option that would work for both properties and fields is runtime Expression compilation. Again,
		// AOT platforms do not support that although that may change according to this thread:
		// https://forum.unity.com/threads/are-c-expression-trees-or-ilgenerator-allowed-on-ios.489498/

		// Regardless, looking up lists and dictionaries requires allocation of an adapter class so it will always
		// be better from a performance perspective to implement IVariableStore directly.

		public string Name => _field != null ? _field.Name : _property.Name;

		private FieldInfo _field;
		private PropertyInfo _property;
		private bool _isWritable;

		#region Creation

		public static Property Create(FieldInfo info, bool isWritable)
		{
			if (IsSupportedType(info.FieldType))
			{
				return new Property
				{
					_field = info,
					_property = null,
					_isWritable = isWritable
				};
			}
			else if (IsSupportedList(info.FieldType))
			{
				return new Property
				{
					_field = info,
					_property = null,
					_isWritable = false
				};
			}
			else
			{
				return null;
			}
		}

		public static Property Create(PropertyInfo info, bool isWritable)
		{
			if (IsSupportedType(info.PropertyType))
			{
				return new Property
				{
					_field = null,
					_property = info,
					_isWritable = isWritable && info.GetSetMethod(true) != null
				};
			}
			else if (IsSupportedList(info.PropertyType))
			{
				return new Property
				{
					_field = null,
					_property = info,
					_isWritable = false
				};
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region Access

		public VariableValue Get(object obj)
		{
			try
			{
				var value = _field != null
					? _field.GetValue(obj)
					: _property.GetValue(obj);

				if (value is IList list)
					return VariableValue.Create(ListAdapter.Create(list));
				else
					return VariableValue.CreateAny(value);
			}
			catch
			{
			}

			return VariableValue.Empty;
		}

		public SetVariableResult Set(object obj, VariableValue value)
		{
			if (!_isWritable)
				return SetVariableResult.ReadOnly;

			var boxed = value.GetBoxedValue();

			try
			{
				if (_field != null)
					_field.SetValue(obj, boxed);
				else if (_property != null)
					_property.SetValue(obj, boxed);

				return SetVariableResult.Success;
			}
			catch
			{
			}

			return SetVariableResult.TypeMismatch;
		}

		#endregion

		#region Support Testing

		private static bool IsSupportedType(Type type)
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

		private static bool IsSupportedList(Type type)
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
}
