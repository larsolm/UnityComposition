using PiRhoSoft.UtilityEngine;
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

	[Serializable]
	public class TagList : SerializedList<string> { }

	[HelpURL(Composition.DocumentationUrl + "variable-schema")]
	[CreateAssetMenu(menuName = "PiRho Soft/Schema", fileName = "Schema", order = 129)]
	public class VariableSchema : ScriptableObject
	{
		public VariableInitializerType InitializerType = VariableInitializerType.DefaultValue;

		[ListDisplay(EmptyText = "Add tags to differentiate variables in the schema into different catagories (usually for resetting and persistance)")]
		public TagList Tags = new TagList();

		[HideInInspector] [SerializeField] private List<VariableDefinition> _definitions = new List<VariableDefinition>();
		[HideInInspector] [SerializeField] private int _version = 0;

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
