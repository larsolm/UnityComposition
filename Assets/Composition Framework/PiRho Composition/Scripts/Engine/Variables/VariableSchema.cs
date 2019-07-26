using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public interface ISchemaOwner
	{
		VariableSchema Schema { get; }
		void SetupSchema();
	}

	[HelpURL(Composition.DocumentationUrl + "variable-schema")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Schema", fileName = nameof(VariableSchema), order = 112)]
	public class VariableSchema : ScriptableObject
	{
		[Serializable]
		public class Entry
		{
			private const string _invalidInitializerError = "(PCVDII) failed to initialize variable '{0}' using store '{1}': the generated value of type '{2}' does not match the definition";

			public string Tag = string.Empty;
			public Expression Initializer = new Expression();
			public VariableDefinition Definition;

			public Variable GenerateValue(IVariableStore variables)
			{
				if (Initializer != null && Initializer.IsValid)
				{
					var value = Initializer.Execute(variables as Object, variables);

					if (!Definition.IsValid(value))
						return value;

					Debug.LogErrorFormat(_invalidInitializerError, Definition.Name, variables, value.Type);
				}

				return Definition.Generate();
			}
		}

		[Serializable] public class TagList : SerializedList<string> { }
		[Serializable] public class EntryList : SerializedList<Entry> { }

		[Tooltip("The tags available to variables with this variable schema")]
		[List(EmptyLabel = "Add tags to categorize variables (usually for resetting and persistance)", AddCallback = nameof(ValidateTags), RemoveCallback = nameof(ValidateTags))]
		[ChangeTrigger(nameof(ValidateTags))]
		public TagList Tags = new TagList();

		[SerializeField] private EntryList _entries = new EntryList();
		[SerializeField] private int _version = 0;

		public int Version
		{
			get { return _version; }
		}

		public int Count
		{
			get { return _entries.Count; }
		}

		public IReadOnlyList<string> Names
		{
			get => _entries.Select(e => e.Definition.Name).ToList(); // TODO: cache
		}

		public Entry this[int index]
		{
			get { return _entries[index]; }
			set { _entries[index] = value; IncrementVersion(); }
		}

		public bool TryGetIndex(string name, out int index)
		{
			for (var i = 0; i < _entries.Count; i++)
			{
				if (_entries[i].Definition.Name == name)
				{
					index = i;
					return true;
				}
			}

			index = -1;
			return false;
		}

		public bool HasDefinition(string name)
		{
			return TryGetIndex(name, out _);
		}

		public bool AddDefinition(string name, VariableType type)
		{
			if (string.IsNullOrEmpty(name) || HasDefinition(name))
				return false;

			_entries.Add(new Entry() { Definition = new VariableDefinition(name, type) });
			IncrementVersion();
			return true;
		}

		public void RemoveDefinition(int index)
		{
			_entries.RemoveAt(index);
			IncrementVersion();
		}

		private void ValidateTags()
		{
			foreach (var entry in _entries)
			{
				if (!Tags.Contains(entry.Tag))
					entry.Tag = Tags.Count > 0 ? Tags[0] : string.Empty;
			}
		}

		private void IncrementVersion()
		{
			_version++;
		}
	}
}
