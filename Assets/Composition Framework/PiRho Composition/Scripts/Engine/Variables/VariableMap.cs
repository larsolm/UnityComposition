using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PiRhoSoft.Composition
{
	public class MappedVariableAttribute : Attribute
	{
		public bool ReadOnly { get; private set; }

		public MappedVariableAttribute()
		{
			ReadOnly = false;
		}

		public MappedVariableAttribute(bool readOnly)
		{
			ReadOnly = readOnly;
		}
	}

	public class MappedCollectionAttribute : MappedVariableAttribute
	{
		public bool AllowAdd { get; private set; }
		public bool AllowRemove { get; private set; }

		MappedCollectionAttribute() : base(true)
		{
			AllowAdd = true;
			AllowRemove = true;
		}

		MappedCollectionAttribute(bool allowAdd, bool allowRemove) : base(true)
		{
			AllowAdd = allowAdd;
			AllowRemove = allowRemove;
		}
	}

	public abstract class VariableMap
	{
		private static Dictionary<Type, VariableMap> _maps = new Dictionary<Type, VariableMap>();

		private List<string> _names = new List<string>();
		private List<IMappedProperty> _properties = new List<IMappedProperty>();
		private Dictionary<string, int> _map = new Dictionary<string, int>();

		static VariableMap()
		{
			Add(new TransformMap());
		}

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
			try
			{
				return _properties[index].Get(obj);
			}
			catch
			{
				return Variable.Empty;
			}
		}

		private SetVariableResult SetProperty(object obj, int index, Variable value)
		{
			if (!_properties[index].IsWritable)
				return SetVariableResult.ReadOnly;

			try
			{
				_properties[index].Set(obj, value);
				return SetVariableResult.Success;
			}
			catch
			{
				return SetVariableResult.TypeMismatch;
			}
		}

		protected interface IMappedProperty
		{
			bool IsWritable { get; }
			Variable Get(object obj);
			void Set(object obj, Variable value);
		}

		private class PropertyInfoProperty : IMappedProperty
		{
			public PropertyInfo Info;

			public bool IsWritable => Info.GetGetMethod(true) != null;
			public Variable Get(object obj) => Variable.Unbox(Info.GetValue(obj));
			public void Set(object obj, Variable value) => Info.SetValue(obj, value.Box());
		}

		private class FieldInfoProperty : IMappedProperty
		{
			public FieldInfo Info;

			public bool IsWritable => true;
			public Variable Get(object obj) => Variable.Unbox(Info.GetValue(obj));
			public void Set(object obj, Variable value) => Info.SetValue(obj, value.Box());
		}

		protected void AddProperty(PropertyInfo property)
		{
			AddProperty(property.Name, new PropertyInfoProperty { Info = property });
		}

		protected void AddProperty(FieldInfo field)
		{
			AddProperty(field.Name, new FieldInfoProperty { Info = field });
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

		#region Adapters

		private class ListAdapter : IVariableList
		{
			private IList _list;

			private bool _allowSet;
			private bool _allowAdd;
			private bool _allowRemove;

			public ListAdapter(IList list)
			{
				_list = list;

				_allowSet = !list.IsReadOnly;
				_allowAdd = !list.IsReadOnly && !list.IsFixedSize;
				_allowRemove = !list.IsReadOnly && !list.IsFixedSize;
			}

			public int VariableCount => _list.Count;

			public Variable GetVariable(int index)
			{
				if (index >= 0 && index <= VariableCount)
					return Get(index);
				else
					return Variable.Empty;
			}

			public SetVariableResult SetVariable(int index, Variable value)
			{
				if (!_allowSet)
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

			protected Variable Get(int index) => Variable.Unbox(_list[index]);
			protected void Set(int index, Variable value) => _list[index] = value.Box();
			protected bool Add(Variable value) => _list.Add(value.Box()) >= 0;
			protected void Remove(int index) => _list.RemoveAt(index);
		}

		private class DictionaryAdapter : IVariableDictionary
		{
			private IDictionary _dictionary;

			private bool _allowSet;
			private bool _allowAdd;
			private bool _allowRemove;

			public DictionaryAdapter(IDictionary dictionary)
			{
				_dictionary = dictionary;

				_allowSet = !dictionary.IsReadOnly;
				_allowAdd = !dictionary.IsReadOnly && !dictionary.IsFixedSize;
				_allowRemove = !dictionary.IsReadOnly && !dictionary.IsFixedSize;
			}

			public VariableSchema Schema => null;
			public IReadOnlyList<string> VariableNames => (IReadOnlyList<string>)_dictionary.Keys;

			public Variable GetVariable(string name)
			{
				if (_dictionary.Contains(name))
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
				else if (_dictionary.Contains(name))
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

			protected Variable Get(string key) => Variable.Unbox(_dictionary[key]);
			protected void Set(string key, Variable value) => _dictionary[key] = value.Box();
			protected void Add(string key, Variable value) => _dictionary.Add(key, value.Box());
			protected void Remove(string key) => _dictionary.Remove(key);
		}

		#endregion
	}

	public abstract class VariableMap<T> : VariableMap
	{
		private class MappedProperty<ValueType> : IMappedProperty
		{
			public Func<T, ValueType> Getter;
			public Action<T, ValueType> Setter;

			public bool IsWritable => Setter != null;
			public Variable Get(object obj) => Variable.Create(Getter((T)obj));
			public void Set(object obj, Variable value) => Setter((T)obj, value.As<ValueType>());
		}

		protected void AddProperty<ValueType>(string name, Func<T, ValueType> getter, Action<T, ValueType> setter)
		{
			AddProperty(name, new MappedProperty<ValueType> { Getter = getter, Setter = setter });
		}
	}
}