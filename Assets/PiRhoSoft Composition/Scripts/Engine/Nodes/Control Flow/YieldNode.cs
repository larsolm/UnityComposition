using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Yield", 23)]
	[HelpURL(Composition.DocumentationUrl + "yield-node")]
	public class YieldNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield return null;
			graph.GoTo(Next, nameof(Next));
		}
	}
}
