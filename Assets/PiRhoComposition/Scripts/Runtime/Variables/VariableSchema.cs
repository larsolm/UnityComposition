using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "variable-schema")]
	[CreateAssetMenu(menuName = "PiRho Composition/Variable Schema", fileName = nameof(VariableSchema), order = 112)]
	public class VariableSchema : ScriptableObject
	{
		public const string EntriesField = nameof(_entries);

		[Serializable] public class EntryList : SerializedList<VariableSchemaEntry> { }

		[SerializeField] private EntryList _entries = new EntryList();
		[HideInInspector] [SerializeField] private List<string> _names = new List<string>();

		public IReadOnlyList<string> Names => _names;

		public int EntryCount => _entries.Count;
		public bool HasEntry(string name) => TryGetEntry(name, out _);
		public VariableSchemaEntry GetEntry(string name) => TryGetEntry(name, out var entry) ? entry : null;
		public VariableSchemaEntry GetEntry(int index) => TryGetEntry(index, out var entry) ? entry : null;

		public bool TryGetEntry(string name, out VariableSchemaEntry entry)
		{
			if (TryGetIndex(name, out var index))
			{
				entry = _entries[index];
				return true;
			}

			entry = null;
			return false;
		}

		public bool TryGetEntry(int index, out VariableSchemaEntry entry)
		{
			if (index >= 0 && index < _entries.Count)
			{
				entry = _entries[index];
				return true;
			}

			entry = null;
			return false;
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

		public bool AddEntry(string name)
		{
			if (!string.IsNullOrEmpty(name) && !HasEntry(name))
			{
				_entries.Add(new VariableSchemaEntry()
				{
					Type = VariableSchemaInitializerType.Default,
					Default = new SerializedVariable { Variable = Variable.Empty },
					Initializer = new Expression(),
					Definition = new VariableDefinition(name, VariableType.Empty),
				});

				EntriesChanged();
				return true;
			}

			return false;
		}

		public bool RemoveEntry(int index)
		{
			if (index >= 0 && index < _entries.Count)
			{
				_entries.RemoveAt(index);
				EntriesChanged();
				return true;
			}

			return false;
		}

		public bool MoveEntry(int from, int to)
		{
			if (from >= 0 && from < _entries.Count && to >= 0 && to < _entries.Count)
			{
				var entry = _entries[from];
				_entries.RemoveAt(from);
				_entries.Insert(to, entry);

				EntriesChanged();
				return true;
			}

			return false;
		}

		private void EntriesChanged()
		{
			_names = _entries.Select(entry => entry.Definition.Name).ToList();
		}
	}

	public enum VariableSchemaInitializerType
	{
		Default,
		Initializer
	}

	[Serializable]
	public class VariableSchemaEntry
	{
		private const string _invalidInitializerError = "(PCVSEII) failed to initialize variable '{0}' using collection '{1}': the generated variable is type '{2}' and does not match the definition '{3}'";

		public VariableSchemaInitializerType Type;
		public Expression Initializer;
		public SerializedVariable Default;
		public VariableDefinition Definition;

		public Variable GenerateVariable(IVariableMap variables)
		{
			if (Type == VariableSchemaInitializerType.Default)
				return Default.Variable;

			if (Initializer != null && Initializer.IsValid)
			{
				var value = Initializer.Execute(variables as Object, variables);

				if (Definition.IsValid(value))
					return value;

				Debug.LogErrorFormat(_invalidInitializerError, Definition.Name, variables, value.Type, Definition.Description);
			}

			return Definition.Generate();
		}
	}
}