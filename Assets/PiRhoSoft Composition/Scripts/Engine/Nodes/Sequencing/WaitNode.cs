using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Wait", 300)]
	[HelpURL(Composition.DocumentationUrl + "wait-node")]
	public class WaitNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The amount of time (in seconds) to wait")]
		[InlineDisplay(PropagateLabel = true)]
		public FloatVariableSource Time = new FloatVariableSource(1.0f);

		[Tooltip("Time is affected by Time.timeScale")]
		public bool UseScaledTime = true;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
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
