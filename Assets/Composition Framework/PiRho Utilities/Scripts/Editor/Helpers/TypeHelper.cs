using System;

namespace PiRhoSoft.Utilities.Editor
{
	public static class TypeHelper
	{
		#region Attributes

		public static bool HasAttribute<AttributeType>(Type type) where AttributeType : Attribute
		{
			return GetAttribute<AttributeType>(type) != null;
		}

		public static AttributeType GetAttribute<AttributeType>(Type type) where AttributeType : Attribute
		{
			var attributes = type.GetCustomAttributes(typeof(AttributeType), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;
		}

		#endregion

		#region Creation

		public static T CreateInstance<T>(Type type) where T : class
		{
			if (IsCreatableAs(typeof(T), type))
				return Activator.CreateInstance(type) as T;

			return null;
		}

		public static bool IsCreatableAs<BaseType>(Type type)
		{
			return IsCreatableAs(typeof(BaseType), type);
		}

		public static bool IsCreatableAs(Type baseType, Type type)
		{
			return baseType.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
		}

		#endregion

	}
}