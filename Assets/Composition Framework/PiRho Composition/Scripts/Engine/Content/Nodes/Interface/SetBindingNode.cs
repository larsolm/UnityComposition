using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "set-binding-node")]
	[CreateGraphNodeMenu("Interface/Set Binding", 300)]
	public class SetBindingNode : GraphNode
	{
		[Tooltip("The node to go to once the binding is set")]
		public GraphNode Next = null;

		[Tooltip("The Binding Root to set bindings on")]
		[VariableReference(typeof(BindingRoot))]
		public VariableReference Object = new VariableReference();

		[Tooltip("The Variable store to set the bindings to")]
		public VariableReference Binding = new VariableReference();

		public override Color NodeColor => Colors.Interface;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Object, out BindingRoot root))
			{
				root.Value = Binding.GetValue(variables);
				VariableBinding.UpdateBinding(root.gameObject, null, null);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
