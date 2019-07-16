using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Sequencing/Wait", 300)]
	[HelpURL(Composition.DocumentationUrl + "wait-node")]
	public class WaitNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The amount of time (in seconds) to wait")]
		[Inline]
		public FloatVariableSource Time = new FloatVariableSource(1.0f);

		[Tooltip("Time is affected by Time.timeScale")]
		public bool UseScaledTime = true;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, Time, out var time))
			{
				if (UseScaledTime)
					yield return new WaitForSeconds(time);
				else
					yield return new WaitForSecondsRealtime(time);
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
