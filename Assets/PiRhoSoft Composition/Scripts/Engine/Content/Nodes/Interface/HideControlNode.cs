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

		[Tooltip("The InterfaceControl to hide")]
		[VariableConstraint(typeof(InterfaceControl))]
		public VariableReference Control = new VariableReference();

		public override Color NodeColor => Colors.InterfaceDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out InterfaceControl control))
				control.Deactivate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
