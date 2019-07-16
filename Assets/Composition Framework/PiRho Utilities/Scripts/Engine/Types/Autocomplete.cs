using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
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
