using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class VariableMap
	{
		private const string _duplicateVariableError = "(CVMDV) Failed to map variable '{0}': a variable with that name already exists in the map";

		public int Version { get; private set; }
		private Dictionary<string, int> _indices = new Dictionary<string, int>();
		private List<string> _names = new List<string>();

		public int Count => _indices.Count;
		public bool Contains(string name) => _indices.ContainsKey(name);
		public int GetIndex(string name) => _indices.TryGetValue(name, out int index) ? index : -1;
		public bool TryGetIndex(string name, out int index) => _indices.TryGetValue(name, out index);
		public IList<string> GetNames() => _names;

		public VariableMap(int version)
		{
			Version = version;
		}

		public VariableMap Add(VariableSchema schema)
		{
			for (var i = 0; i < schema.Count; i++)
				Add(schema[i].Name);

			return this;
		}

		public VariableMap Add(PropertyMap map)
		{
			for (var i = 0; i < map.Properties.Count; i++)
				Add(map.Properties[i].Name);

			return this;
		}

		private void Add(string name)
		{
			if (!_indices.ContainsKey(name))
			{
				_indices.Add(name, _indices.Count);
				_names.Add(name);
			}
			else
			{
				Debug.LogErrorFormat(_duplicateVariableError, name);
			}
		}
	}
}
