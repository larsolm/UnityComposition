using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Wait", 100)]
	[HelpURL(Composition.DocumentationUrl + "wait-node")]
	public class WaitNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The amount of time (in seconds) to wait")]
		[Minimum(0.0f)]
		public float Time = 1.0f;

		public override Color NodeColor => Colors.Sequencing;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield return new WaitForSeconds(Time);

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
