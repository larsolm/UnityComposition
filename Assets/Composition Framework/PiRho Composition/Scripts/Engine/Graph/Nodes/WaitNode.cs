using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Wait", 24)]
	[HelpURL(Configuration.DocumentationUrl + "wait-node")]
	public class WaitNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The amount of time (in seconds) to wait")]
		public FloatVariableSource Time = new FloatVariableSource(1.0f);

		[Tooltip("Time is affected by Time.timeScale")]
		public bool UseScaledTime = true;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.Resolve(this, Time, out var time))
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
