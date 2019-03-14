using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "update-binding-node")]
	[CreateInstructionGraphNodeMenu("Interface/Update Binding", 300)]
	public class UpdateBindingNode : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(CUBNMO) failed to update bindings: unable to find object '{0}'";
		private const string _invalidObjectWarning = "(CUBNIO) failed to update bindings: '{0}' is not a GameObject, InterfaceControl, or Component";

		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Object to update bindings for")]
		[VariableConstraint(typeof(Object))]
		public VariableReference Object = new VariableReference();

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Group;

		[Tooltip("Wether to wait for any possible animations the bindings will perform")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Interface;

		private BindingAnimationStatus _status = new BindingAnimationStatus();

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			_status.Reset();

			if (Resolve<Object>(variables, Object, out var obj))
			{
				if (variables.Root is GameObject gameObject)
				{
					VariableBinding.UpdateBinding(gameObject, Group, _status);
				}
				else if (variables.Root is InterfaceControl control)
				{
					VariableBinding.UpdateBinding(control.gameObject, Group, _status);

					foreach (var dependency in control.DependentObjects)
						VariableBinding.UpdateBinding(dependency, Group, _status);
				}
				else if (variables.Root is Component component)
				{
					VariableBinding.UpdateBinding(component.gameObject, Group, _status);
				}
			}

			if (WaitForCompletion)
			{
				while (!_status.IsFinished())
					yield return null;
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
