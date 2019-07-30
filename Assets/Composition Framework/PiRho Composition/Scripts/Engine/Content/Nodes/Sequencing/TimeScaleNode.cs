using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Sequencing/Time Scale", 250)]
	[HelpURL(Configuration.DocumentationUrl + "time-scale-node")]
	public class TimeScaleNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The value to set the TimeScale to")]
		[Inline]
		[VariableReference(0.0f, 100.0f)]
		public FloatVariableSource TimeScale = new FloatVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (Resolve(variables, TimeScale, out var time))
				Time.timeScale = time;

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
