using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Break", 22)]
	[HelpURL(Composition.DocumentationUrl + "break-node")]
	public class BreakNode : InstructionGraphNode
	{
		[Tooltip("The node to go to next")]
		public InstructionGraphNode Next = null;

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			graph.BreakTo(Next);
			yield break;
		}
	}
}
