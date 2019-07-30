using PiRhoSoft.Composition;
using System.Collections;

namespace PiRhoSoft.CompositionExample
{
	[CreateGraphNodeMenu("PiRho Soft/Examples/Autocomplete")]
	public class AutocompleteNode : GraphNode
	{
		public VariableReference Variable = new VariableReference();

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			yield break;
		}
	}
}
