using PiRhoSoft.CompositionEngine;
using System.Collections;

namespace PiRhoSoft.CompositionExample
{
	[CreateInstructionGraphNodeMenu("PiRho Soft/Examples/Autocomplete")]
	public class AutocompleteNode : InstructionGraphNode
	{
		public VariableReference Variable = new VariableReference();

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield break;
		}
	}
}
