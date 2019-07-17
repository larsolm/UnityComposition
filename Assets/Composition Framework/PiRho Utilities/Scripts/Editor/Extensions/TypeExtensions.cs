using System;

namespace PiRhoSoft.Utilities.Editor
{
	public static class TypeExtensions
	{
		#region Attributes

		public static bool HasAttribute<AttributeType>(this Type type) where AttributeType : Attribute
		{
			return GetAttribute<AttributeType>(type) != null;
		}

		public static AttributeType GetAttribute<AttributeType>(this Type type) where AttributeType : Attribute
		{
			var attributes = type.GetCustomAttributes(typeof(AttributeType), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;
		}

		#endregion

		#region Creation

		public static T CreateInstance<T>(this Type type) where T : class
		{
			if (IsCreatableAs(type, typeof(T)))
				return Activator.CreateInstance(type) as T;

			return null;
		}

		public static bool IsCreatableAs<BaseType>(this Type type)
		{
			return IsCreatableAs(type, typeof(BaseType));
		}

		public static bool IsCreatableAs(this Type type, Type baseType)
		{
			return baseType.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
		}

		#endregion

	}
}