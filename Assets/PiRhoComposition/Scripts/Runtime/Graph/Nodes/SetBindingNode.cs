using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Assets.PiRhoComposition.Scripts.Runtime.Graph.Nodes
{
	[CreateGraphNodeMenu("Bindings/Set Binding", 0)]
	[HelpURL(Configuration.DocumentationUrl + "set-binding-node")]
	public class SetBindingNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Binding Root to set the new binding on")]
		[VariableConstraint(typeof(BindingRoot))]
		public VariableLookupReference Binding = new VariableLookupReference();

		[Tooltip("The Variable to set as the binding")]
		public VariableValueSource Value = new VariableValueSource();

		public override Color NodeColor => Colors.Interface;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<BindingRoot>(this, Binding, out var binding) && variables.Resolve(this, Value, out var value))
				binding.Value.Variable = value;

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
