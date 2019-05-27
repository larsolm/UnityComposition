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

		[Tooltip("The InterfaceControl to show")]
		[VariableConstraint(typeof(InterfaceControl))]
		public VariableReference Control = new VariableReference();

		public override Color NodeColor => Colors.InterfaceLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out InterfaceControl control))
				control.Activate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
