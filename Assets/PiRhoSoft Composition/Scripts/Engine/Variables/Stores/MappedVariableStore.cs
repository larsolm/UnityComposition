using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class MappedVariableAttribute : Attribute
	{
		public bool Readable;
		public bool Writable;

		public MappedVariableAttribute()
		{
			Readable = true;
			Writable = true;
		}

		public MappedVariableAttribute(bool readable, bool writable)
		{
			Readable = readable;
			Writable = writable;
		}
	}

	public class MappedVariableStore : IVariableStore, IVariableList
	{
		private const string _invalidFieldError = "(CMVSIF) failed to map field '{0}' of type '{1}': Only bool, int, float, string, Object derived, or IVariableStore derived field types can be mapped";
		private const string _invalidPropertyError = "(CMVSIP) failed to map property '{0}' of type '{1}': Only bool, int, float, string, Object derived, or IVariableStore derived property types can be mapped";

		private VariableMap _map;
		private IVariableList[] _lists;

		public void Setup<OwnerType>(OwnerType owner, VariableSchema schema, VariableList variables) where OwnerType : class
		{
			var mapping = GetMapping(owner, schema);
			var listCount = mapping.Properties.Count;

			if (schema != null && variables != null)
				listCount++;

			var lists = new IVariableList[listCount];

			for (var i = 0; i < mapping.Properties.Count; i++)
				lists[i] = mapping.Properties[i].CreateList(owner);

			if (variables != null)
			{
				if (schema != null)
				{
					if (owner is IVariableListener listener)
						lists[listCount - 1] = new VariableListener(listener, variables);
					else
						lists[listCount - 1] = variables;

					variables.Setup(schema, owner as IVariableStore);
				}
				else
				{
					variables.Clear();
				}
			}

			Setup(mapping.Map, lists);
		}

		public void Setup(VariableMap map, params IVariableList[] lists)
		{
			_map = map;
			_lists = lists;
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			return _map != null && _map.TryGetIndex(name, out int index)
				? GetVariableValue(index)
				: VariableValue.Empty;
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			return _map != null && _map.TryGetIndex(name, out int index)
				? SetVariableValue(index, value)
				: SetVariableResult.NotFound;
		}

		public IEnumerable<string> GetVariableNames()
		{
			return _map.GetNames();
		}

		#endregion

		#region IVariableList Implementation

		public int VariableCount
		{
			get { return _map != null ? _map.Count : 0; }
		}

		public string GetVariableName(int index)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.GetVariableName(index);

					index -= list.VariableCount;
				}
			}

			return null;
		}

		public VariableValue GetVariableValue(int index)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.GetVariableValue(index);

					index -= list.VariableCount;
				}
			}

			return VariableValue.Empty;
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			if (_lists != null)
			{
				for (var i = 0; i < _lists.Length; i++)
				{
					var list = _lists[i];

					if (index < list.VariableCount)
						return list.SetVariableValue(index, value);

					index -= list.VariableCount;
				}
			}

			return SetVariableResult.NotFound;
		}

		#endregion

		#region Setup

		private class MappingData
		{
			public VariableMap Map;
			public List<PropertyMap> Properties;
		}

		private static Dictionary<Tuple<Type, VariableSchema>, MappingData> _mappingDatas = new Dictionary<Tuple<Type, VariableSchema>, MappingData>();
		private static Dictionary<Type, PropertyMap> _propertyMaps = new Dictionary<Type, PropertyMap>();

		private MappingData GetMapping<OwnerType>(OwnerType owner, VariableSchema schema) where OwnerType : class
		{
			var key = Tuple.Create(typeof(OwnerType), schema);
			var version = schema != null ? schema.Version : 0;

			if (!_mappingDatas.TryGetValue(key, out var data) || data.Map.Version != version)
			{
				if (data == null)
				{
					data = new MappingData
					{
						Properties = GetPropertyMaps<OwnerType>()
					};

					_mappingDatas.Add(key, data);
				}

				data.Map = new VariableMap(version);

				foreach (var propertyMap in data.Properties)
					data.Map.Add(propertyMap);

				if (schema != null)
					data.Map.Add(schema);
			}

			return data;
		}

		private List<PropertyMap> GetPropertyMaps<OwnerType>() where OwnerType : class
		{
			var maps = new List<PropertyMap>();
			var type = typeof(OwnerType);

			while (type != null && type != typeof(MonoBehaviour) && type != typeof(object))
			{
				// when using Delegate.CreateDelegate, the object type in the method signature can be a more derived
				// type than the declaring type, but not a less derived type

				var map = GetPropertyMap<OwnerType>(type);

				if (map != null)
					maps.Add(map);

				type = type.BaseType;
			}

			return maps;
		}

		private PropertyMap GetPropertyMap<OwnerType>(Type type) where OwnerType : class
		{
			if (!_propertyMaps.TryGetValue(type, out var propertyMap))
			{
				var map = new PropertyMap<OwnerType>();
				FindProperties(type, map);

				propertyMap = map;
				_propertyMaps.Add(type, propertyMap);
			}

			return propertyMap;
		}

		private void FindProperties<OwnerType>(Type type, PropertyMap<OwnerType> map) where OwnerType : class
		{
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (var field in fields)
			{
				var mapping = field.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
				{
					Func<OwnerType, VariableValue> getter = null;
					Func<OwnerType, VariableValue, SetVariableResult> setter = null;

					if (field.FieldType == typeof(bool))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, bool>(field) : null;
						setter = mapping.Writable ? CreateBoolSetter<OwnerType>(field) : null;
					}
					else if (field.FieldType == typeof(int))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, int>(field) : null;
						setter = mapping.Writable ? CreateIntSetter<OwnerType>(field) : null;
					}
					else if (field.FieldType == typeof(float))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, float>(field) : null;
						setter = mapping.Writable ? CreateFloatSetter<OwnerType>(field) : null;
					}
					else if (field.FieldType == typeof(string))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, string>(field) : null;
						setter = mapping.Writable ? CreateStringSetter<OwnerType>(field) : null;
					}
					else if (typeof(Object).IsAssignableFrom(field.FieldType))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, Object>(field) : null;
						setter = mapping.Writable ? CreateObjectSetter<OwnerType>(field) : null;
					}
					else if (typeof(IVariableStore).IsAssignableFrom(field.FieldType))
					{
						getter = mapping.Readable ? CreateGetter<OwnerType, IVariableStore>(field) : null;
						setter = mapping.Writable ? CreateStoreSetter<OwnerType>(field) : null;
					}
					else
					{
						Debug.LogErrorFormat(_invalidFieldError, field.Name, field.FieldType.Name);
						continue;
					}

					map.Add(field.Name, getter, setter);
				}
			}

			foreach (var property in properties)
			{
				var mapping = property.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
				{
					var getMethod = mapping.Readable ? property.GetGetMethod(true) : null;
					var setMethod = mapping.Writable ? property.GetSetMethod(true) : null;

					Func<OwnerType, VariableValue> getter = null;
					Func<OwnerType, VariableValue, SetVariableResult> setter = null;

					if (property.PropertyType == typeof(bool))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, bool>(getMethod) : null;
						setter = setMethod != null ? CreateBoolSetter<OwnerType>(setMethod) : null;
					}
					else if (property.PropertyType == typeof(int))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, int>(getMethod) : null;
						setter = setMethod != null ? CreateIntSetter<OwnerType>(setMethod) : null;
					}
					else if (property.PropertyType == typeof(float))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, float>(getMethod) : null;
						setter = setMethod != null ? CreateFloatSetter<OwnerType>(setMethod) : null;
					}
					else if (property.PropertyType == typeof(string))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, string>(getMethod) : null;
						setter = setMethod != null ? CreateStringSetter<OwnerType>(setMethod) : null;
					}
					else if (typeof(Object).IsAssignableFrom(property.PropertyType))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, Object>(getMethod) : null;
						setter = setMethod != null ? CreateObjectSetter<OwnerType>(property.PropertyType, setMethod) : null;
					}
					else if (typeof(IVariableStore).IsAssignableFrom(property.PropertyType))
					{
						getter = getMethod != null ? CreateGetter<OwnerType, IVariableStore>(getMethod) : null;
						setter = setMethod != null ? CreateStoreSetter<OwnerType>(property.PropertyType, setMethod) : null;
					}
					else
					{
						Debug.LogErrorFormat(_invalidPropertyError, property.Name, property.PropertyType.Name);
						continue;
					}

					map.Add(property.Name, getter, setter);
				}
			}
		}

		#region Field Accessors

		private Func<OwnerType, VariableValue> CreateGetter<OwnerType, PropertyType>(FieldInfo field)
		{
			// TODO: look in to the expression building stuff which is probably platform dependent if Unity supports it at all

			return obj =>
			{
				var value = (PropertyType)field.GetValue(obj);
				return VariableValue.Create(value);
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateBoolSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if (value.Type == VariableType.Boolean)
				{
					field.SetValue(obj, value.Boolean);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateIntSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if (value.Type == VariableType.Integer)
				{
					field.SetValue(obj, value.Integer);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateFloatSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if (value.Type == VariableType.Number || value.Type == VariableType.Integer)
				{
					field.SetValue(obj, value.Number);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateStringSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if (value.Type == VariableType.String)
				{
					field.SetValue(obj, value.String);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateObjectSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if ((value.Type == VariableType.Object || value.Type == VariableType.Store) && field.FieldType.IsAssignableFrom(value.RawObject.GetType()))
				{
					field.SetValue(obj, value.RawObject);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}


		private Func<OwnerType, VariableValue, SetVariableResult> CreateStoreSetter<OwnerType>(FieldInfo field)
		{
			return (obj, value) =>
			{
				if ((value.Type == VariableType.Object || value.Type == VariableType.Store) && field.FieldType.IsAssignableFrom(value.RawObject.GetType()))
				{
					field.SetValue(obj, value.RawObject);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		#endregion

		#region Property Accessors

		private Func<OwnerType, VariableValue> CreateGetter<OwnerType, PropertyType>(MethodInfo getter)
		{
			var caller = (Func<OwnerType, PropertyType>)Delegate.CreateDelegate(typeof(Func<OwnerType, PropertyType>), getter);

			return obj =>
			{
				var value = caller(obj);
				return VariableValue.Create(value);
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateBoolSetter<OwnerType>(MethodInfo setter)
		{
			var caller = (Action<OwnerType, bool>)Delegate.CreateDelegate(typeof(Action<OwnerType, bool>), setter);

			return (obj, value) =>
			{
				if (value.Type == VariableType.Boolean)
				{
					caller(obj, value.Boolean);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateIntSetter<OwnerType>(MethodInfo setter)
		{
			var caller = (Action<OwnerType, int>)Delegate.CreateDelegate(typeof(Action<OwnerType, int>), setter);

			return (obj, value) =>
			{
				if (value.Type == VariableType.Integer)
				{
					caller(obj, value.Integer);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateFloatSetter<OwnerType>(MethodInfo setter)
		{
			var caller = (Action<OwnerType, float>)Delegate.CreateDelegate(typeof(Action<OwnerType, float>), setter);

			return (obj, value) =>
			{
				if (value.Type == VariableType.Number || value.Type == VariableType.Integer)
				{
					caller(obj, value.Number);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateStringSetter<OwnerType>(MethodInfo setter)
		{
			var caller = (Action<OwnerType, string>)Delegate.CreateDelegate(typeof(Action<OwnerType, string>), setter);

			return (obj, value) =>
			{
				if (value.Type == VariableType.String)
				{
					caller(obj, value.String);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private static object[] _setParameters = new object[1];

		private Func<OwnerType, VariableValue, SetVariableResult> CreateObjectSetter<OwnerType>(Type objectType, MethodInfo setter)
		{
			return (obj, value) =>
			{
				if ((value.Type == VariableType.Object || value.Type == VariableType.Store) && objectType.IsAssignableFrom(value.RawObject.GetType()))
				{
					_setParameters[0] = value.RawObject;
					setter.Invoke(obj, _setParameters);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		private Func<OwnerType, VariableValue, SetVariableResult> CreateStoreSetter<OwnerType>(Type storeType, MethodInfo setter)
		{
			return (obj, value) =>
			{
				if ((value.Type == VariableType.Object || value.Type == VariableType.Store) && storeType.IsAssignableFrom(value.RawObject.GetType()))
				{
					_setParameters[0] = value.RawObject;
					setter.Invoke(obj, _setParameters);
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.TypeMismatch;
				}
			};
		}

		#endregion

		#endregion
	}
}
