using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class StateAutocomplete : AutocompleteSource
	{
		private static List<AutocompleteItem> _states = new List<AutocompleteItem>
		{
			new AutocompleteItem { Name = "Alabama", Source = null },
			new AutocompleteItem { Name = "Alaska", Source = null },
			new AutocompleteItem { Name = "Arizona", Source = null },
			new AutocompleteItem { Name = "Arkansas", Source = null },
			new AutocompleteItem { Name = "California", Source = null },
			new AutocompleteItem { Name = "Colorado", Source = null },
			new AutocompleteItem { Name = "Connecticut", Source = null },
			new AutocompleteItem { Name = "Delaware", Source = null },
			new AutocompleteItem { Name = "Florida", Source = null },
			new AutocompleteItem { Name = "Georgia", Source = null },
			new AutocompleteItem { Name = "Hawaii", Source = null },
			new AutocompleteItem { Name = "Idaho", Source = null },
			new AutocompleteItem { Name = "Illinois", Source = null },
			new AutocompleteItem { Name = "Indiana", Source = null },
			new AutocompleteItem { Name = "Iowa", Source = null },
			new AutocompleteItem { Name = "Kansas", Source = null },
			new AutocompleteItem { Name = "Kentucky", Source = null },
			new AutocompleteItem { Name = "Louisiana", Source = null },
			new AutocompleteItem { Name = "Maine", Source = null },
			new AutocompleteItem { Name = "Maryland", Source = null },
			new AutocompleteItem { Name = "Massachusetts", Source = null },
			new AutocompleteItem { Name = "Michigan", Source = null },
			new AutocompleteItem { Name = "Minnesota", Source = null },
			new AutocompleteItem { Name = "Mississippi", Source = null },
			new AutocompleteItem { Name = "Missouri", Source = null },
			new AutocompleteItem { Name = "Montana", Source = null },
			new AutocompleteItem { Name = "Nebraska", Source = null },
			new AutocompleteItem { Name = "Nevada", Source = null },
			new AutocompleteItem { Name = "New Hampshire", Source = null },
			new AutocompleteItem { Name = "New Jersey", Source = null },
			new AutocompleteItem { Name = "New Mexico", Source = null },
			new AutocompleteItem { Name = "New York", Source = null },
			new AutocompleteItem { Name = "North Carolina", Source = null },
			new AutocompleteItem { Name = "North Dakota", Source = null },
			new AutocompleteItem { Name = "Ohio", Source = null },
			new AutocompleteItem { Name = "Oklahoma", Source = null },
			new AutocompleteItem { Name = "Oregon", Source = null },
			new AutocompleteItem { Name = "Pennsylvania", Source = null },
			new AutocompleteItem { Name = "Rhode Island", Source = null },
			new AutocompleteItem { Name = "South Carolina", Source = null },
			new AutocompleteItem { Name = "South Dakota", Source = null },
			new AutocompleteItem { Name = "Tennessee", Source = null },
			new AutocompleteItem { Name = "Texas", Source = null },
			new AutocompleteItem { Name = "Utah", Source = null },
			new AutocompleteItem { Name = "Vermont", Source = null },
			new AutocompleteItem { Name = "Virginia", Source = null },
			new AutocompleteItem { Name = "Washington", Source = null },
			new AutocompleteItem { Name = "West Virginia", Source = null },
			new AutocompleteItem { Name = "Wisconsin", Source = null },
			new AutocompleteItem { Name = "Wyoming", Source = null }
		};

		public override List<AutocompleteItem> Items => _states;
	}

	public class Autocomplete : MonoBehaviour
	{
		[Autocomplete(typeof(StateAutocomplete))]
		public string State;
	}
}