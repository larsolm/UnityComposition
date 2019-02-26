using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "update-binding-node")]
	[CreateInstructionGraphNodeMenu("Interface/Update Binding", 201)]
	public class UpdateBindingNode : InstructionGraphNode, IImmediate
	{
		private const string _invalidThisError = "(CUBNIT) failed to update bindings: {0} is not an IVariableStore";

		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		[Tooltip("The control to update")]
		public InterfaceReference Control = new InterfaceReference();

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Bindings;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var control = Control.GetControl<InterfaceControl>();

			if (control)
			{
				if (variables.This is IVariableStore store)
					control.UpdateBindings(store, Bindings);
				else
					Debug.LogErrorFormat(this, _invalidThisError, This);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
