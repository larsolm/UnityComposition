using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public interface IAutocompleteSource
	{
		AutocompleteSource AutocompleteSource { get; }
	}

	public class AutocompleteItem
	{
		public string Name;
		public AutocompleteSource Source;
	}

	public class AutocompleteIndex : IEnumerable<AutocompleteItem>
	{
		private SortedList<string, AutocompleteItem> _items = new SortedList<string, AutocompleteItem>();

		public void Add(string name, AutocompleteSource source) => _items.Add(name, new AutocompleteItem { Name = name, Source = source });
		public AutocompleteItem Get(string name) => _items.TryGetValue(name, out var value) ? value : null;

		public IEnumerator<AutocompleteItem> GetEnumerator() => _items.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public abstract class AutocompleteSource
	{
		public abstract List<AutocompleteItem> Items { get; }
	}

	public class AutocompleteAttribute : PropertyAttribute
	{
		public Type SourceType { get; private set; }

		public AutocompleteAttribute(Type sourceType) => SourceType = sourceType;
	}
}
