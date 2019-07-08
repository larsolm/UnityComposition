using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Sequencing/Time Scale", 250)]
	[HelpURL(Composition.DocumentationUrl + "time-scale-node")]
	public class TimeScaleNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The value to set the TimeScale to")]
		[Inline]
		[VariableConstraint(0.0f, 100.0f)]
		public FloatVariableSource TimeScale = new FloatVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, TimeScale, out var time))
				Time.timeScale = time;

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
