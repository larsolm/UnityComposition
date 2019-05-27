using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Exit", 23)]
	[HelpURL(Composition.DocumentationUrl + "exit-node")]
	public class ExitNode : InstructionGraphNode
	{
		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			graph.BreakAll();
			yield break;
		}
	}
}
