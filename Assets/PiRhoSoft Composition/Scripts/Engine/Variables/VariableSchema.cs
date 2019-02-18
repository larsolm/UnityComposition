using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum VariableInitializerType
	{
		Expression,
		DefaultValue,
		None
	}

	public class VariableInitializerAttribute : Attribute
	{
		public VariableInitializerType Type;
		public VariableInitializerAttribute(VariableInitializerType type) => Type = type;
	}

	public class VariableAvailabilitiesAttribute : Attribute
	{
		public string[] Availabilities;
		public VariableAvailabilitiesAttribute(params string[] availabilities) => Availabilities = availabilities;
	}

	[Serializable]
	public class VariableSchema
	{
		[SerializeField] private List<VariableDefinition> _definitions = new List<VariableDefinition>();
		[SerializeField] private int _version = 0;

		public int Version
		{
			get { return _version; }
		}

		public int Count
		{
			get { return _definitions.Count; }
		}

		public VariableDefinition this[int index]
		{
			get { return _definitions[index]; }
			set { _definitions[index] = value; IncrementVersion(); }
		}

		public int GetIndex(string name)
		{
			for (var i = 0; i < _definitions.Count; i++)
			{
				if (_definitions[i].Name == name)
					return i;
			}

			return -1;
		}

		public bool HasDefinition(string name)
		{
			return GetIndex(name) >= 0;
		}

		public bool AddDefinition(string name, VariableType type)
		{
			if (string.IsNullOrEmpty(name) || HasDefinition(name))
				return false;

			_definitions.Add(VariableDefinition.Create(name, type, VariableDefinition.NotSaved));
			IncrementVersion();
			return true;
		}

		public void RemoveDefinition(int index)
		{
			_definitions.RemoveAt(index);
			IncrementVersion();
		}

		private void IncrementVersion()
		{
			_version++;
		}
	}
}
