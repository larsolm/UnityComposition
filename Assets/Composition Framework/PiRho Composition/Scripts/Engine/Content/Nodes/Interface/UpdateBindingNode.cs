using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "update-binding-node")]
	[CreateGraphNodeMenu("Interface/Update Binding", 301)]
	public class UpdateBindingNode : GraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public GraphNode Next = null;

		[Tooltip("The Object to update bindings for")]
		[VariableReference(typeof(Object))]
		public VariableReference Object = new VariableReference();

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Group;

		[Tooltip("Wether to wait for any possible animations the bindings will perform")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Interface;

		private BindingAnimationStatus _status = new BindingAnimationStatus();

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			_status.Reset();

			if (ResolveObject(variables, Object, out Object obj))
			{
				if (obj is GameObject gameObject)
				{
					VariableBinding.UpdateBinding(gameObject, Group, WaitForCompletion ? _status : null);
				}
				else if (obj is InterfaceControl control)
				{
					VariableBinding.UpdateBinding(control.gameObject, Group, WaitForCompletion ? _status : null);

					foreach (var dependency in control.DependentObjects)
						VariableBinding.UpdateBinding(dependency, Group, WaitForCompletion ? _status : null);
				}
				else if (obj is Component component)
				{
					VariableBinding.UpdateBinding(component.gameObject, Group, WaitForCompletion ? _status : null);
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
