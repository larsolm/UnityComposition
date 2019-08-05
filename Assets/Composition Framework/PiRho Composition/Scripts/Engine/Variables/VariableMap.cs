using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class MappedVariableAttribute : Attribute
	{
		public bool AllowSet { get; private set; }

		public MappedVariableAttribute()
		{
			AllowSet = true;
		}

		public MappedVariableAttribute(bool allowSet)
		{
			AllowSet = allowSet;
		}
	}

	public class MappedCollectionAttribute : MappedVariableAttribute
	{
		public bool AllowAdd { get; private set; }
		public bool AllowRemove { get; private set; }
		public bool AllowChange { get; private set; }

		MappedCollectionAttribute() : base(false)
		{
			AllowAdd = true;
			AllowRemove = true;
			AllowChange = true;
		}

		MappedCollectionAttribute(bool allowAdd, bool allowRemove, bool allowChange) : base(false)
		{
			AllowAdd = allowAdd;
			AllowRemove = allowRemove;
			AllowChange = allowChange;
		}

		MappedCollectionAttribute(bool allowAdd, bool allowRemove, bool allowChange, bool allowSet) : base(allowSet)
		{
			AllowAdd = allowAdd;
			AllowRemove = allowRemove;
			AllowChange = allowChange;
		}
	}

	public abstract class VariableMap
	{
		private static Dictionary<Type, VariableMap> _maps = new Dictionary<Type, VariableMap>();

		private List<string> _names = new List<string>();
		private List<IMappedProperty> _properties = new List<IMappedProperty>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		public static void Add<T>(VariableMap<T> map)
		{
			_maps.Add(typeof(T), map);
		}

		public static VariableMap Get(Type type)
		{
			if (!_maps.TryGetValue(type, out VariableMap map) && Configuration.AutoCreateVariableMaps)
			{
				map = new ReflectedMap(type);
				_maps.Add(type, map);
			}

			return map;
		}

		public static bool TryGet(Type type, out VariableMap map)
		{
			return _maps.TryGetValue(type, out map);
		}

		public int VariableCount
		{
			get => _properties.Count;
		}

		public Variable GetVariable(object obj, int index)
		{
			return index >= 0 && index < _properties.Count
				? GetProperty(obj, index)
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(object obj, int index, Variable value)
		{
			return index >= 0 && index < _properties.Count
				? SetProperty(obj, index, value)
				: SetVariableResult.NotFound;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => _names;
		}

		public Variable GetVariable(object obj, string name)
		{
			return _map.TryGetValue(name, out int index)
				? GetProperty(obj, index)
				: Variable.Empty;
		}

		public SetVariableResult SetVariable(object obj, string name, Variable value)
		{
			return _map.TryGetValue(name, out int index)
				? SetProperty(obj, index, value)
				: SetVariableResult.NotFound;
		}

		private Variable GetProperty(object obj, int index)
		{
			try { return _properties[index].Get(obj); }
			catch { }
			return Variable.Empty;
		}

		private SetVariableResult SetProperty(object obj, int index, Variable value)
		{
			try { return _properties[index].Set(obj, value); }
			catch { }
			return SetVariableResult.TypeMismatch;
		}

		protected interface IMappedProperty
		{
			Variable Get(object obj);
			SetVariableResult Set(object obj, Variable value);
		}

		protected void AddProperty(PropertyInfo property)
		{
			if (IsList(property.PropertyType))
				AddProperty(property.Name, new ListPropertyInfoProperty(property));
			if (IsDictionary(property.PropertyType))
				AddProperty(property.Name, new DictionaryPropertyInfoProperty(property));
			else
				AddProperty(property.Name, new PropertyInfoProperty(property));
		}

		protected void AddProperty(FieldInfo field)
		{
			if (IsList(field.FieldType))
				AddProperty(field.Name, new ListFieldInfoProperty(field));
			if (IsDictionary(field.FieldType))
				AddProperty(field.Name, new DictionaryFieldInfoProperty(field));
			else
				AddProperty(field.Name, new FieldInfoProperty(field));
		}

		protected void AddProperty(string name, IMappedProperty property)
		{
			_map.Add(name, _properties.Count);
			_names.Add(name);
			_properties.Add(property);
		}

		protected void AddProperties(VariableMap map)
		{
			for (var i = 0; i < map._properties.Count; i++)
				AddProperty(map._names[i], map._properties[i]);
		}

		private bool IsList(Type type) => type.GetInterfaces().Contains(typeof(IList));
		private bool IsDictionary(Type type) => type.GetInterfaces().Contains(typeof(IDictionary));

		#region Mapped Property Types

		private class PropertyInfoProperty : IMappedProperty
		{
			public PropertyInfo Info { get; private set; }
			public bool AllowSet { get; private set; }

			public PropertyInfoProperty(PropertyInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedVariableAttribute>();

				Info = info;
				AllowSet = (attribute == null || attribute.AllowSet) && info.GetSetMethod(true) != null;
			}

			public Variable Get(object obj)
			{
				var value = Info.GetValue(obj);
				return Variable.Unbox(value);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (AllowSet)
				{
					Info.SetValue(obj, value.Box());
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.ReadOnly;
				}
			}
		}

		private class ListPropertyInfoProperty : IMappedProperty
		{
			public PropertyInfo Info { get; private set; }

			public bool AllowSet { get; private set; }
			public bool AllowAdd { get; private set; }
			public bool AllowRemove { get; private set; }
			public bool AllowChange { get; private set; }

			public ListPropertyInfoProperty(PropertyInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedCollectionAttribute>();

				Info = info;
				AllowSet = attribute != null && attribute.AllowSet;
				AllowAdd = attribute == null || attribute.AllowAdd;
				AllowRemove = attribute == null || attribute.AllowRemove;
				AllowChange = attribute == null || attribute.AllowChange;
			}

			public Variable Get(object obj)
			{
				var value = (IList)Info.GetValue(obj);
				var list = new ListAdapter(value, AllowAdd, AllowRemove, AllowChange);
				return Variable.List(list);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (!AllowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (value.AsList is ListAdapter list)
				{
					Info.SetValue(obj, list.List);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			}
		}

		private class DictionaryPropertyInfoProperty : IMappedProperty
		{
			public PropertyInfo Info { get; private set; }

			public bool AllowSet { get; private set; }
			public bool AllowAdd { get; private set; }
			public bool AllowRemove { get; private set; }
			public bool AllowChange { get; private set; }

			public DictionaryPropertyInfoProperty(PropertyInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedCollectionAttribute>();

				Info = info;
				AllowSet = attribute != null && attribute.AllowSet;
				AllowAdd = attribute == null || attribute.AllowAdd;
				AllowRemove = attribute == null || attribute.AllowRemove;
				AllowChange = attribute == null || attribute.AllowChange;
			}

			public Variable Get(object obj)
			{
				var value = (IDictionary)Info.GetValue(obj);
				var dictionary = new DictionaryAdapter(value, AllowAdd, AllowRemove, AllowChange);
				return Variable.Dictionary(dictionary);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (!AllowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (value.AsDictionary is DictionaryAdapter dictionary)
				{
					Info.SetValue(obj, dictionary.Dictionary);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			}
		}

		private class FieldInfoProperty : IMappedProperty
		{
			public FieldInfo Info { get; private set; }
			public bool AllowSet { get; private set; }

			public FieldInfoProperty(FieldInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedVariableAttribute>();

				Info = info;
				AllowSet = (attribute == null || attribute.AllowSet) && !info.IsInitOnly && !info.IsLiteral;
			}

			public Variable Get(object obj)
			{
				var value = Info.GetValue(obj);
				return Variable.Unbox(value);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (AllowSet)
				{
					Info.SetValue(obj, value.Box());
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.ReadOnly;
				}
			}
		}

		private class ListFieldInfoProperty : IMappedProperty
		{
			public FieldInfo Info { get; private set; }

			public bool AllowSet { get; private set; }
			public bool AllowAdd { get; private set; }
			public bool AllowRemove { get; private set; }
			public bool AllowChange { get; private set; }

			public ListFieldInfoProperty(FieldInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedCollectionAttribute>();

				Info = info;
				AllowSet = attribute != null && attribute.AllowSet && !info.IsInitOnly && !info.IsLiteral;
				AllowAdd = attribute == null || attribute.AllowAdd;
				AllowRemove = attribute == null || attribute.AllowRemove;
				AllowChange = attribute == null || attribute.AllowChange;
			}

			public Variable Get(object obj)
			{
				var value = (IList)Info.GetValue(obj);
				var list = new ListAdapter(value, AllowAdd, AllowRemove, AllowChange);
				return Variable.List(list);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (!AllowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (value.AsList is ListAdapter list)
				{
					Info.SetValue(obj, list.List);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			}
		}

		private class DictionaryFieldInfoProperty : IMappedProperty
		{
			public FieldInfo Info { get; private set; }

			public bool AllowSet { get; private set; }
			public bool AllowAdd { get; private set; }
			public bool AllowRemove { get; private set; }
			public bool AllowChange { get; private set; }

			public DictionaryFieldInfoProperty(FieldInfo info)
			{
				var attribute = info.GetCustomAttribute<MappedCollectionAttribute>();

				Info = info;
				AllowSet = attribute != null && attribute.AllowSet && !info.IsInitOnly && !info.IsLiteral;
				AllowAdd = attribute == null || attribute.AllowAdd;
				AllowRemove = attribute == null || attribute.AllowRemove;
				AllowChange = attribute == null || attribute.AllowChange;
			}

			public Variable Get(object obj)
			{
				var value = (IDictionary)Info.GetValue(obj);
				var dictionary = new DictionaryAdapter(value, AllowAdd, AllowRemove, AllowChange);
				return Variable.Dictionary(dictionary);
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (!AllowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (value.AsDictionary is DictionaryAdapter dictionary)
				{
					Info.SetValue(obj, dictionary.Dictionary);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			}
		}

		#endregion

		#region Adapter Types

		protected class ListAdapter : IVariableList
		{
			public IList List;

			private bool _allowAdd;
			private bool _allowRemove;
			private bool _allowChange;

			public ListAdapter(IList list, bool allowAdd, bool allowRemove, bool allowChange)
			{
				List = list;

				_allowAdd = allowAdd && !list.IsReadOnly && !list.IsFixedSize;
				_allowRemove = allowRemove && !list.IsReadOnly && !list.IsFixedSize;
				_allowChange = allowChange && !list.IsReadOnly;
			}

			public int VariableCount => List.Count;

			public Variable GetVariable(int index)
			{
				if (index >= 0 && index <= VariableCount)
					return Get(index);
				else
					return Variable.Empty;
			}

			public SetVariableResult SetVariable(int index, Variable value)
			{
				if (!_allowChange)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (index >= 0 && index <= VariableCount)
				{
					try
					{
						Set(index, value);
						return SetVariableResult.Success;
					}
					catch
					{
						return SetVariableResult.TypeMismatch;
					}
				}

				return SetVariableResult.NotFound;
			}

			public SetVariableResult AddVariable(Variable value)
			{
				if (!_allowAdd)
					return SetVariableResult.ReadOnly;

				try
				{
					if (Add(value))
						return SetVariableResult.Success;
				}
				catch
				{
				}

				return SetVariableResult.TypeMismatch;
			}

			public SetVariableResult RemoveVariable(int index)
			{
				if (!_allowRemove)
					return SetVariableResult.ReadOnly;

				if (index >= 0 && index <= VariableCount)
				{
					Remove(index);
					return SetVariableResult.Success;
				}

				return SetVariableResult.NotFound;
			}

			protected Variable Get(int index) => Variable.Unbox(List[index]);
			protected void Set(int index, Variable value) => List[index] = value.Box();
			protected bool Add(Variable value) => List.Add(value.Box()) >= 0;
			protected void Remove(int index) => List.RemoveAt(index);
		}

		protected class DictionaryAdapter : IVariableDictionary
		{
			public IDictionary Dictionary;

			private bool _allowAdd;
			private bool _allowRemove;
			private bool _allowSet;

			public DictionaryAdapter(IDictionary dictionary, bool allowAdd, bool allowRemove, bool allowSet)
			{
				Dictionary = dictionary;

				_allowAdd = allowAdd && !dictionary.IsReadOnly && !dictionary.IsFixedSize;
				_allowRemove = allowRemove && !dictionary.IsReadOnly && !dictionary.IsFixedSize;
				_allowSet = allowSet && !dictionary.IsReadOnly;
			}

			public VariableSchema Schema => null;
			public IReadOnlyList<string> VariableNames => (IReadOnlyList<string>)Dictionary.Keys;

			public Variable GetVariable(string name)
			{
				if (Dictionary.Contains(name))
					return Get(name);
				else
					return Variable.Empty;
			}

			public SetVariableResult SetVariable(string name, Variable value)
			{
				if (!_allowSet)
				{
					return SetVariableResult.ReadOnly;
				}
				else if (Dictionary.Contains(name))
				{
					try
					{
						Set(name, value);
						return SetVariableResult.Success;
					}
					catch
					{
						return SetVariableResult.TypeMismatch;
					}
				}

				return SetVariableResult.NotFound;
			}

			protected Variable Get(string key) => Variable.Unbox(Dictionary[key]);
			protected void Set(string key, Variable value) => Dictionary[key] = value.Box();
			protected void Add(string key, Variable value) => Dictionary.Add(key, value.Box());
			protected void Remove(string key) => Dictionary.Remove(key);
		}

		#endregion

		#region Map Types

		private class ReflectedMap : VariableMap
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

		#endregion
	}

	public abstract class VariableMap<T> : VariableMap
	{
		protected void AddValue<ValueType>(string name, Func<T, ValueType> getter, Action<T, ValueType> setter)
		{
			AddProperty(name, new MappedValue<ValueType> { Getter = getter, Setter = setter });
		}

		protected void AddList<ListType>(string name, Func<T, ListType> getter, Action<T, ListType> setter = null, bool allowAdd = true, bool allowRemove = true, bool allowChange = true) where ListType : IList
		{
			AddProperty(name, new MappedList<ListType> { Getter = getter, Setter = setter, AllowAdd = allowAdd, AllowRemove = allowRemove, AllowChange = allowChange });
		}

		protected void AddDictionary<DictionaryType>(string name, Func<T, DictionaryType> getter, Action<T, DictionaryType> setter = null, bool allowAdd = true, bool allowRemove = true, bool allowChange = true) where DictionaryType : IDictionary
		{
			AddProperty(name, new MappedDictionary<DictionaryType> { Getter = getter, Setter = setter, AllowAdd = allowAdd, AllowRemove = allowRemove, AllowChange = allowChange });
		}

		#region Mapped Property Types

		private class MappedValue<ValueType> : IMappedProperty
		{
			public Func<T, ValueType> Getter;
			public Action<T, ValueType> Setter;

			public Variable Get(object obj)
			{
				return Variable.Create(Getter((T)obj));
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (Setter == null)
					return SetVariableResult.ReadOnly;

				Setter((T)obj, value.As<ValueType>());
				return SetVariableResult.Success;
			}
		}

		private class MappedList<ListType> : IMappedProperty where ListType : IList
		{
			public Func<T, ListType> Getter;
			public Action<T, ListType> Setter;

			public bool AllowAdd;
			public bool AllowRemove;
			public bool AllowChange;

			public Variable Get(object obj)
			{
				var list = Getter((T)obj);
				return Variable.List(new ListAdapter(list, AllowAdd, AllowRemove, AllowChange));
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (Setter == null)
					return SetVariableResult.ReadOnly;

				if (value.AsList is ListAdapter list)
				{
					Setter((T)obj, (ListType)list.List);
					return SetVariableResult.Success;
				}

				return SetVariableResult.TypeMismatch;
			}
		}

		private class MappedDictionary<DictionaryType> : IMappedProperty where DictionaryType : IDictionary
		{
			public Func<T, DictionaryType> Getter;
			public Action<T, DictionaryType> Setter;

			public bool AllowAdd;
			public bool AllowRemove;
			public bool AllowChange;

			public Variable Get(object obj)
			{
				var dictionary = Getter((T)obj);
				return Variable.Dictionary(new DictionaryAdapter(dictionary, AllowAdd, AllowRemove, AllowChange));
			}

			public SetVariableResult Set(object obj, Variable value)
			{
				if (Setter == null)
					return SetVariableResult.ReadOnly;

				if (value.AsDictionary is DictionaryAdapter dictionary)
				{
					Setter((T)obj, (DictionaryType)dictionary.Dictionary);
					return SetVariableResult.Success;
				}

				return SetVariableResult.TypeMismatch;
			}
		}

		#endregion
	}
}