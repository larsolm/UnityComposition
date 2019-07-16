using PiRhoSoft.Composition;
using System.Collections;

namespace PiRhoSoft.CompositionExample
{
	[CreateGraphNodeMenu("PiRho Soft/Examples/Autocomplete")]
	public class AutocompleteNode : GraphNode
	{
		public VariableReference Variable = new VariableReference();

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			yield break;
		}
	}
}
