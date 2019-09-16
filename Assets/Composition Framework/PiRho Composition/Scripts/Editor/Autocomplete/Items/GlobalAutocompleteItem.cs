using System.Collections.Generic;

namespace PiRhoSoft.Composition.Editor
{
	public class GlobalAutocompleteItem : AutocompleteItem
	{
		public void Setup(IList<VariableDefinition> globals)
		{
			// globals is globals set on the graph; still enumerate VariableLinks
		}

		public override void Setup(object obj)
		{
		}
	}
}
