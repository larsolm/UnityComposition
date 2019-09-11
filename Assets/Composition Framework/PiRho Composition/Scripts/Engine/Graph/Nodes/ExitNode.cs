using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Exit", 23)]
	[HelpURL(Configuration.DocumentationUrl + "exit-node")]
	public class ExitNode : GraphNode
	{
		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			// graph.Exit(); TODO: need to do something like execute GraphRunners on the CompositionManager, store
			// each of the resulting coroutines on the graph, then cancel them all and make sure they clean up
			// correctly
			yield break;
		}
	}
}
