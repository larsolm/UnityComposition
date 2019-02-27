using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Interface/Clear Transition", 201)]
	[HelpURL(Composition.DocumentationUrl + "clear-transition")]
	public class ClearTransition : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color GetNodeColor()
		{
			return new Color(0.0f, 0.25f, 0.0f);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			TransitionManager.Instance.EndTransition();

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
