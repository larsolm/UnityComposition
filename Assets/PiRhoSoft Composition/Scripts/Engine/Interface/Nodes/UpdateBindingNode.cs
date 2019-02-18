using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "update-binding-node")]
	[CreateInstructionGraphNodeMenu("Interface/Update Binding", 201)]
	public class UpdateBindingNode : InstructionGraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		[Tooltip("The control to update")]
		public InterfaceReference Control = new InterfaceReference();

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Bindings;

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var control = Control.GetControl<InterfaceControl>();

			if (control)
				control.UpdateBindings(variables.This, Bindings);

			graph.GoTo(Next);

			yield break;
		}
	}
}
