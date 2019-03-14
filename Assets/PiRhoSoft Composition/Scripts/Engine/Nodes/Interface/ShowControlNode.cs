using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "show-control-node")]
	[CreateInstructionGraphNodeMenu("Interface/Show Control", 101)]
	public class ShowControlNode : InstructionGraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.InterfaceLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			//Control.Activate(this);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
