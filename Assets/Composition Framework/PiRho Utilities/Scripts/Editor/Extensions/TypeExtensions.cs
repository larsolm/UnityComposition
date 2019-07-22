using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public static class TypeExtensions
	{
		#region Attributes

		public static bool HasAttribute<AttributeType>(this Type type) where AttributeType : Attribute
		{
			return type.GetAttribute<AttributeType>() != null;
		}

		public static AttributeType GetAttribute<AttributeType>(this Type type) where AttributeType : Attribute
		{
			return TryGetAttribute<AttributeType>(type, out var attribute) ? attribute : null;
		}

		public static bool TryGetAttribute<AttributeType>(this Type type, out AttributeType attribute) where AttributeType : Attribute
		{
			var attributes = type.GetCustomAttributes(typeof(AttributeType), false);
			attribute = attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;

			return attribute != null;
		}

		#endregion

		#region Creation

		public static T CreateInstance<T>(this Type type) where T : class
		{
			if (type.IsCreatableAs(typeof(T)))
				return Activator.CreateInstance(type) as T;

			return null;
		}

		public static bool IsCreatableAs<BaseType>(this Type type)
		{
			return type.IsCreatableAs(typeof(BaseType));
		}

		public static bool IsCreatableAs(this Type type, Type baseType)
		{
			return baseType.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
		}

		#endregion

		#region Serialization

		// these functions are based on Unity's serialization rules defined here: https://docs.unity3d.com/Manual/script-Serialization.html

		public static List<Type> SerializableTypes = new List<Type>
		{
			typeof(bool),
			typeof(sbyte), typeof(short), typeof(int), typeof(long),
			typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
			typeof(float), typeof(double), typeof(decimal),
			typeof(char), typeof(string),
			typeof(Vector2), typeof(Vector3), typeof(Vector4),
			typeof(Quaternion), typeof(Matrix4x4),
			typeof(Color), typeof(Color32), typeof(Gradient),
			typeof(Rect), typeof(RectOffset),
			typeof(LayerMask), typeof(AnimationCurve), typeof(GUIStyle)
		};

		public static bool IsSerializable(this Type type)
		{
			return type.IsSerializable(false);
		}

		private static bool IsSerializable(this Type type, bool inner)
		{
			if (type.IsAbstract)
				return false; // covers static as well

			if (type.IsEnum)
				return true;

			if (type.IsGenericType)
				return !inner && type.GetGenericTypeDefinition() == typeof(List<>) && IsSerializable(type.GetGenericArguments()[0], true);

			if (type.IsArray && type.GetElementType().IsSerializable(true))
				return !inner;

			if (typeof(Object).IsAssignableFrom(type))
				return true;

			if (type.HasAttribute<SerializableAttribute>())
				return true;

			return SerializableTypes.Contains(type);
		}

		#endregion
	}
}