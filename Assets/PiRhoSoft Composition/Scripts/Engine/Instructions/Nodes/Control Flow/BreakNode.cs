using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Break", 22)]
	[HelpURL(Composition.DocumentationUrl + "break-node")]
	public class BreakNode : InstructionGraphNode, IImmediate
	{
		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			graph.Break();
			yield break;
		}
	}
}
