using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Break", 22)]
	[HelpURL(Composition.DocumentationUrl + "break-node")]
	public class BreakNode : InstructionGraphNode
	{
		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			graph.Break();
			yield break;
		}
	}
}
