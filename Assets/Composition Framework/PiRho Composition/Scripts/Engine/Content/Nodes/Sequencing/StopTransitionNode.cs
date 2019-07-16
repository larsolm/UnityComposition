using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Sequencing/Stop Transition", 1)]
	[HelpURL(Composition.DocumentationUrl + "stop-transition-node")]
	public class StopTransitionNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			TransitionManager.Instance.EndTransition();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
