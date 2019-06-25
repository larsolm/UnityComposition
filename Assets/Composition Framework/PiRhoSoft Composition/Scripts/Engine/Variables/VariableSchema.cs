using PiRhoSoft.PargonUtilities.Engine;
using PiRhoSoft.UtilityEngine;
using System;
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

	public interface ISchemaOwner
	{
		VariableSchema Schema { get; }
		void SetupSchema();
	}

	[HelpURL(Composition.DocumentationUrl + "variable-schema")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Schema", fileName = nameof(VariableSchema), order = 129)]
	public class VariableSchema : ScriptableObject
	{
		public VariableInitializerType InitializerType = VariableInitializerType.DefaultValue;

		[List(EmptyLabel = "Add tags to categorize variables (usually for resetting and persistance)")]
		public TagList Tags = new TagList();

		[HideInInspector] [SerializeField] private VariableDefinitionList _definitions = new VariableDefinitionList();
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

			_definitions.Add(new VariableDefinition { Name = name, Definition = ValueDefinition.Create(type, null) });
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
