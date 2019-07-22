using System;
using System.Reflection;

namespace PiRhoSoft.Utilities.Editor
{
	public static class FieldInfoExtensions
	{
		#region Attributes

		public static bool HasAttribute<AttributeType>(this FieldInfo field) where AttributeType : Attribute
		{
			return field.GetCustomAttribute<AttributeType>() != null;
		}

		public static AttributeType GetAttribute<AttributeType>(this FieldInfo field) where AttributeType : Attribute
		{
			return TryGetAttribute<AttributeType>(field, out var attribute) ? attribute : null;
		}

		public static bool TryGetAttribute<AttributeType>(this FieldInfo field, out AttributeType attribute) where AttributeType : Attribute
		{
			var attributes = field.GetCustomAttributes(typeof(AttributeType), false);
			attribute = attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;

			return attribute != null;
		}

		#endregion
	}
}