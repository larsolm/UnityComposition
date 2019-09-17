using PiRhoSoft.Composition;
using System.Collections;

namespace PiRhoSoft.CompositionExample
{
	[CreateGraphNodeMenu("PiRho Soft/Examples/Autocomplete")]
	public class AutocompleteNode : GraphNode
	{
		public VariableLookupReference Variable = new VariableLookupReference();

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			yield break;
		}
	}
}
