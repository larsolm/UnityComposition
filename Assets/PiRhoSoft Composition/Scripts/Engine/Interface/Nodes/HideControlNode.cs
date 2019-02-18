using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "hide-control-node")]
	[CreateInstructionGraphNodeMenu("Interface/Hide Control", 102)]
	public class HideControlNode : InstructionGraphNode
	{
		[Tooltip("The node to go to once the control is hidden")]
		public InstructionGraphNode Next = null;

		[Tooltip("The control to hide")]
		public InterfaceReference Control = new InterfaceReference();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Control.Deactivate();
			graph.GoTo(Next);
			yield break;
		}
	}
}
