using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal interface IMappedVariableList
	{
		int VariableCount { get; }
		string GetVariableName(int index);
		VariableValue GetVariableValue(int index);
		SetVariableResult SetVariableValue(int index, VariableValue value);
	}

	public interface IVariableListener
	{
		void VariableChanged(int index, VariableValue value);
	}

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

	public class MappedVariableStore : IVariableStore, IMappedVariableList
	{
		private const string _invalidFieldError = "(CMVSIF) failed to map field '{0}' of type '{1}': Only VariableValue types can be mapped";
		private const string _invalidPropertyError = "(CMVSIP) failed to map property '{0}' of type '{1}': Only VariableValue types can be mapped";

		private VariableMap _map;
		private IMappedVariableList[] _lists;

		public void Setup(object owner, VariableSchema schema, VariableSet variables)
		{
			var mapping = GetMapping(owner.GetType(), schema);
			var listCount = mapping.Properties.Count;

			if (schema != null && variables != null)
				listCount++;

			var lists = new IMappedVariableList[listCount];

			for (var i = 0; i < mapping.Properties.Count; i++)
				lists[i] = new PropertyList(owner, mapping.Properties[i]);

			// variable initializers may need the map set to access other variables
			_map = mapping.Map;
			_lists = lists;

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

		public IList<string> GetVariableNames()
		{
			return _map.GetNames();
		}

		#endregion

		#region IMappedVariableList Implementation

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

		private MappingData GetMapping(Type ownerType, VariableSchema schema)
		{
			var key = Tuple.Create(ownerType, schema);
			var version = schema != null ? schema.Version : 0;

			if (!_mappingDatas.TryGetValue(key, out var data) || data.Map.Version != version)
			{
				if (data == null)
				{
					data = new MappingData
					{
						Properties = GetPropertyMaps(ownerType)
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

		private List<PropertyMap> GetPropertyMaps(Type ownerType)
		{
			var maps = new List<PropertyMap>();
			var type = ownerType;

			while (type != null && type != typeof(MonoBehaviour) && type != typeof(ScriptableObject) && type != typeof(object))
			{
				var map = GetPropertyMap(type);

				if (map != null)
					maps.Add(map);

				type = type.BaseType;
			}

			return maps;
		}

		private PropertyMap GetPropertyMap(Type type)
		{
			if (!_propertyMaps.TryGetValue(type, out var map))
			{
				map = new PropertyMap(type);
				_propertyMaps.Add(type, map);
			}

			return map;
		}

		#endregion
	}
}
