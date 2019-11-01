using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Assets.PiRhoComposition.Scripts.Runtime.Graph.Nodes
{
	[CreateGraphNodeMenu("Bindings/Update Bindings", 1)]
	[HelpURL(Configuration.DocumentationUrl + "update-bindings-node")]
	public class UpdateBindingsNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The GameObject to update the bindings on")]
		[VariableConstraint(typeof(GameObject))]
		public VariableLookupReference Bindings = new VariableLookupReference();

		[Tooltip("The group of bindings to update (empty means all)")]
		public StringVariableSource Group = new StringVariableSource(string.Empty);

		[Tooltip("Whether to wait for any binding animations to complete before moving to the next node")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Interface;

		private BindingAnimationStatus _status = new BindingAnimationStatus();

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<GameObject>(this, Bindings, out var bindings))
			{
				_status.Reset();

				variables.Resolve(this, Group, out var group);
				VariableBinding.UpdateBinding(bindings, group, _status);

				if (WaitForCompletion)
				{
					while (!_status.IsFinished())
						yield return null;
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
