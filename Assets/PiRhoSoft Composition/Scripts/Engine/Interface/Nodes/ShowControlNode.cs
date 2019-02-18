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

		[Tooltip("The control to show")]
		public InterfaceReference Control = new InterfaceReference();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Control.Activate();
			graph.GoTo(Next);
			yield break;
		}
	}
}
