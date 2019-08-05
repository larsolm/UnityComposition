using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class TreeAutocomplete : AutocompleteSource
	{
		private static readonly List<AutocompleteItem> _tree = new List<AutocompleteItem>
		{
			new AutocompleteItem { Name = "One", IsArray = false, Source = new StaticTreeAutocomplete() },
			new AutocompleteItem { Name = "Two", IsArray = true, Source = new TreeAutocomplete() },
			new AutocompleteItem { Name = "Three", IsArray = true, Source = null },
			new AutocompleteItem { Name = "Four", IsArray = false, Source = null },
		};

		public override bool SupportsCustom => true;
		public override List<AutocompleteItem> Items => _tree;
	}

	public class StaticTreeAutocomplete : AutocompleteSource
	{
		private static readonly List<AutocompleteItem> _tree = new List<AutocompleteItem>
		{
			new AutocompleteItem { Name = "One Fish", IsArray = false, Source = new TreeAutocomplete() },
			new AutocompleteItem { Name = "Two Fish", IsArray = true, Source = new StaticTreeAutocomplete() },
			new AutocompleteItem { Name = "Red Fish", IsArray = true, Source = null },
			new AutocompleteItem { Name = "Blue Fish", IsArray = false, Source = null },
		};

		public override bool SupportsCustom => false;
		public override List<AutocompleteItem> Items => _tree;
	}
}