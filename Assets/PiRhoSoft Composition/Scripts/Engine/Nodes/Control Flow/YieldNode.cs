using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Yield", 23)]
	[HelpURL(Composition.DocumentationUrl + "yield-node")]
	public class YieldNode : InstructionGraphNode
	{
		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield return null;
		}
	}
}
