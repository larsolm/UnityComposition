using System;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class ReflectedMap : VariableMap
	{
		public ReflectedMap(Type type)
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

			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (var info in fields)
			{
				var mapping = info.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
					AddProperty(info);
			}

			foreach (var info in properties)
			{
				var mapping = info.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
					AddProperty(info);
			}

			type = type.BaseType;

			while (type != null && type != typeof(MonoBehaviour) && type != typeof(ScriptableObject) && type != typeof(object))
			{
				var map = Get(type);
				AddProperties(map);
				type = type.BaseType;
			}
		}
	}
}