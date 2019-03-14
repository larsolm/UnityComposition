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

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Group;

		[Tooltip("Wether to wait for any possible animations the bindings will perform")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Interface;

		private BindingAnimationStatus _status = new BindingAnimationStatus();

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			_status.Reset();

			if (variables.This is GameObject obj)
			{
				VariableBinding.UpdateBinding(obj, Group, _status);
			}
			else if (variables.This is InterfaceControl control)
			{
				VariableBinding.UpdateBinding(control.gameObject, Group, _status);

				foreach (var dependency in control.DependentObjects)
					VariableBinding.UpdateBinding(dependency, Group, _status);
			}
			else if (variables.This is Component component)
			{
				VariableBinding.UpdateBinding(component.gameObject, Group, _status);
			}
			else
			{
				if (variables.This == null)
					Debug.LogWarningFormat(this, _missingObjectWarning, This);
				else
					Debug.LogWarningFormat(this, _invalidObjectWarning, This);

				yield break;
			}

			if (WaitForCompletion)
			{
				while (!_status.IsFinished())
					yield return null;
			}

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
