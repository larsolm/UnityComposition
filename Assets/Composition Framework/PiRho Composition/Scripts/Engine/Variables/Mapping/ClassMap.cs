using System;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IClassMap
	{
		IList<string> GetVariableNames();
		Variable GetVariable(object obj, string name);
		SetVariableResult SetVariable(object obj, string name, Variable value);
	}

	public static class ClassMap
	{
		private static Dictionary<Type, IClassMap> _maps = new Dictionary<Type, IClassMap>();

		static ClassMap()
		{
			Add(new TransformMap());
			Add(new CameraMap());
		}

		public static void Add<T>(ClassMap<T> map)
		{
			_maps.Add(typeof(T), map);
		}

		public static bool Get(Type type, out IClassMap map)
		{
			return _maps.TryGetValue(type, out map);
		}
	}

	public abstract class ClassMap<T> : IClassMap
	{
		public abstract IList<string> GetVariableNames();
		public abstract Variable GetVariable(T obj, string name);
		public abstract SetVariableResult SetVariable(T obj, string name, Variable value);

		public Variable GetVariable(object obj, string name)
		{
			if (obj is T t)
				return GetVariable(t, name);
			else
				return Variable.Empty;
		}

		public SetVariableResult SetVariable(object obj, string name, Variable value)
		{
			if (obj is T t)
				return SetVariable(t, name, value);
			else
				return SetVariableResult.NotFound;
		}
	}
}
