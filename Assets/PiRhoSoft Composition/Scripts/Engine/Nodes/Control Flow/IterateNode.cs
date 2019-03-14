using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : InstructionGraphNode, ILoopNode
	{
		[Tooltip("The Indexed Variable Store containing the objects to iterate")]
		public VariableReference Container;

		[Tooltip("The node to go to for each object in the iteration")]
		public InstructionGraphNode Loop = null;

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveOther<IIndexedVariableStore>(variables, Container, out var store))
			{
				var item = store.GetItem(iteration);

				if (Loop != null && item != null)
				{
					graph.ChangeRoot(item);
					graph.GoTo(Loop, nameof(Loop));
				}
			}

			yield break;
		}
	}
}
