using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PiRhoSoft.Utilities.Editor
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

		public static List<Type> ListDerivedTypes<BaseType>(bool includeAbstract)
		{
			return ListDerivedTypes(typeof(BaseType), includeAbstract);
		}

		public static List<Type> ListDerivedTypes(Type baseType, bool includeAbstract)
		{
			return TypeCache.GetTypesDerivedFrom(baseType.BaseType).Where(type => includeAbstract ? baseType.IsAssignableFrom(type) : !type.IsNestedPrivate && type.IsCreatableAs(baseType)).ToList();
		}

		public static IEnumerable<Type> FindTypes(Func<Type, bool> predicate)
		{
			// There are a lot of assemblies so it might make sense to filter the list a bit. There isn't a specific
			// way to do that, but something like this would work: https://stackoverflow.com/questions/5160051/c-sharp-how-to-get-non-system-assemblies

			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => !assembly.IsDynamic) // GetExportedTypes throws an exception when called on dynamic assemblies
				.SelectMany(t => t.GetExportedTypes())
				.Where(predicate);
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
	}
}
