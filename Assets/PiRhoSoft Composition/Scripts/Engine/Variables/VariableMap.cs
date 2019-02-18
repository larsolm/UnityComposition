using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class VariableMap
	{
		private const string _duplicateVariableError = "(CVMDV) Failed to map variable '{0}': a variable with that name already exists in the map";

		public int Version { get; private set; }
		private Dictionary<string, int> _indices = new Dictionary<string, int>();

		public int Count => _indices.Count;
		public bool Contains(string name) => _indices.ContainsKey(name);
		public int GetIndex(string name) => _indices.TryGetValue(name, out int index) ? index : -1;
		public bool TryGetIndex(string name, out int index) => _indices.TryGetValue(name, out index);

		public VariableMap(int version)
		{
		}

		public VariableMap Add(VariableSchema schema)
		{
			for (var i = 0; i < schema.Count; i++)
				Add(schema[i].Name);

			return this;
		}

		public VariableMap Add(PropertyMap map)
		{
			for (var i = 0; i < map.PropertyCount; i++)
				Add(map.GetPropertyName(i));

			return this;
		}
		
		private void Add(string name)
		{
			if (!_indices.ContainsKey(name))
				_indices.Add(name, _indices.Count);
			else
				Debug.LogErrorFormat(_duplicateVariableError, name);
		}
	}
}
