using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class TypeList
	{
		public TypeList(Type baseType)
		{
			BaseType = baseType;
		}

		public Type BaseType { get; private set; }

		public List<Type> Types;
		public List<string> Paths;
	}

	public static class TypeHelper
	{
		private static Dictionary<string, TypeList> _derivedTypeLists = new Dictionary<string, TypeList>();

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

		public static AttributeType GetAttribute<AttributeType>(FieldInfo field) where AttributeType : Attribute
		{
			var attributes = field.GetCustomAttributes(typeof(AttributeType), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;
		}

		public static bool HasAttribute(Type type, Type attributeType)
		{
			return GetAttribute(type, attributeType) != null;
		}

		public static Attribute GetAttribute(Type type, Type attributeType)
		{
			var attributes = type.GetCustomAttributes(attributeType, false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as Attribute : null;
		}

		public static Attribute GetAttribute(FieldInfo field, Type attributeType)
		{
			var attributes = field.GetCustomAttributes(attributeType, false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as Attribute : null;
		}

		#endregion

		#region Creation

		public static T CreateInstance<T>(Type type) where T : class
		{
			if (type != null && !type.IsAbstract && typeof(T).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
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

		#region Listing

		public static List<Type> ListDerivedTypes<BaseType>(bool includeAbstract)
		{
			return ListDerivedTypes(typeof(BaseType), includeAbstract);
		}

		public static List<Type> ListDerivedTypes(Type baseType, bool includeAbstract)
		{
			return TypeCache.GetTypesDerivedFrom(baseType.BaseType).Where(type => includeAbstract ? baseType.IsAssignableFrom(type) : IsCreatableAs(baseType, type)).ToList();
		}

		public static List<Type> ListTypesWithAttribute<AttributeType>() where AttributeType : Attribute
		{
			return TypeCache.GetTypesWithAttribute<AttributeType>().ToList();
		}

		public static List<Type> ListTypesWithAttribute(Type attributeType)
		{
			return TypeCache.GetTypesWithAttribute(attributeType).ToList();
		}

		public static TypeList GetTypeList<T>(bool includeAbstract)
		{
			return GetTypeList(typeof(T), includeAbstract);
		}

		public static TypeList GetTypeList(Type baseType, bool includeAbstract)
		{
			// include the settings in the name so lists of the same type can be created with different settings
			var listName = string.Format("{0}-{1}", includeAbstract, baseType.AssemblyQualifiedName);

			if (!_derivedTypeLists.TryGetValue(listName, out var typeList))
			{
				typeList = new TypeList(baseType);
				_derivedTypeLists.Add(listName, typeList);
			}

			if (typeList.Types == null)
			{
				var types = ListDerivedTypes(baseType, includeAbstract);
				var ordered = types.Select(type => new PathedType(types, baseType, type)).OrderBy(type => type.Path);

				typeList.Types = ordered.Select(type => type.Type).ToList();
				typeList.Paths = ordered.Select(type => type.Path).ToList();
			}

			return typeList;
		}

		private class PathedType
		{
			public Type Type;
			public string Path;

			public PathedType(IEnumerable<Type> types, Type rootType, Type type)
			{
				Type = type;
				Path = Type.Name;

				// repeat the name for types that have derivations so they appear in their own submenu (otherwise they wouldn't be selectable)
				if (type != rootType)
				{
					if (types.Any(t => t.BaseType == type))
						Path += "/" + Type.Name;

					type = type.BaseType;
				}

				// prepend all parent type names up to but not including the root type
				while (type != rootType)
				{
					Path = type.Name + "/" + Path;
					type = type.BaseType;
				}
			}
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

		public static bool IsSerializable(FieldInfo field)
		{
			var included = field.IsPublic || GetAttribute<SerializeField>(field) != null;
			var excluded = GetAttribute<NonSerializedAttribute>(field) != null;
			var compatible = !field.IsStatic && !field.IsLiteral && !field.IsInitOnly && IsSerializable(field.FieldType);

			return included && !excluded && compatible;
		}

		public static bool IsSerializable(Type type)
		{
			return IsSerializable(type, false);
		}

		private static bool IsSerializable(Type type, bool inner)
		{
			if (type.IsAbstract)
				return false; // covers static as well

			if (type.IsEnum)
				return true;

			if (type.IsGenericType)
				return !inner && type.GetGenericTypeDefinition() == typeof(List<>) && IsSerializable(type.GetGenericArguments()[0], true);

			if (type.IsArray && IsSerializable(type.GetElementType(), true))
				return !inner;

			if (typeof(Object).IsAssignableFrom(type))
				return true;

			if (GetAttribute<SerializableAttribute>(type) != null)
				return true;

			return SerializableTypes.Contains(type);
		}

		#endregion
	}
}
