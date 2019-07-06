using PiRhoSoft.PargonUtilities.Engine;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class InstructionAutocomplete : AutocompleteSource
	{
		private List<AutocompleteItem> _items = new List<AutocompleteItem>();

		public override List<AutocompleteItem> Items => _items;

		public InstructionAutocomplete(Instruction instruction)
		{
			_items.Add(new AutocompleteItem { Name = instruction.ContextName, Source = new VariableDefinitionAutocomplete(instruction.ContextDefinition) });
			_items.Add(new AutocompleteItem { Name = InstructionStore.InputStoreName, Source = new InstructionInputAutocomplete(instruction) });
			_items.Add(new AutocompleteItem { Name = InstructionStore.OutputStoreName, Source = new InstructionOutputAutocomplete(instruction) });
		}
	}

	public class InstructionInputAutocomplete : AutocompleteSource
	{
		private List<AutocompleteItem> _items = new List<AutocompleteItem>();

		public override List<AutocompleteItem> Items => _items;

		public InstructionInputAutocomplete(Instruction instruction)
		{
			foreach (var input in instruction.Inputs)
				_items.Add(new AutocompleteItem { Name = input.Name, Source = new VariableDefinitionAutocomplete(input.Definition) });
		}
	}

	public class InstructionOutputAutocomplete : AutocompleteSource
	{
		private List<AutocompleteItem> _items = new List<AutocompleteItem>();

		public override List<AutocompleteItem> Items => _items;

		public InstructionOutputAutocomplete(Instruction instruction)
		{
			foreach (var output in instruction.Outputs)
				_items.Add(new AutocompleteItem { Name = output.Name, Source = new VariableDefinitionAutocomplete(output.Definition) });
		}
	}

	public class InstructionLocalAutocomplete : AutocompleteSource
	{
		private List<AutocompleteItem> _items = new List<AutocompleteItem>();

		public override List<AutocompleteItem> Items => _items;

		public InstructionLocalAutocomplete(Instruction instruction)
		{
		}
	}

	public class VariableDefinitionAutocomplete : AutocompleteSource
	{
		private List<AutocompleteItem> _items = new List<AutocompleteItem>();

		public override List<AutocompleteItem> Items => _items;

		public VariableDefinitionAutocomplete(ValueDefinition definition)
		{
		}
	}
}
