using System.Collections.Generic;

namespace PiRhoSoft.Utilities
{
	public interface IAutocompleteSource
	{
		AutocompleteSource AutocompleteSource { get; }
	}

	public class AutocompleteItem
	{
		public string Name;
		public bool IsArray;
		public bool SupportsCasting;
		public AutocompleteSource Source;
	}

	public abstract class AutocompleteSource
	{
		public abstract bool SupportsCustom { get; }
		public abstract List<AutocompleteItem> Items { get; }

		public AutocompleteItem GetItem(string name)
		{
			foreach (var item in Items)
			{
				if (item.Name == name)
					return item;
			}

			return null;
		}
	}
}
