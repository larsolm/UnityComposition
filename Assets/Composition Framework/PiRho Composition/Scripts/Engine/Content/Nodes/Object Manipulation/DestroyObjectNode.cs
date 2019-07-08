using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Object Manipulation/Destroy Object", 2)]
	[HelpURL(Composition.DocumentationUrl + "destroy-object-node")]
	public class DestroyObjectNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The target Object to destroy")]
		[VariableConstraint(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Object target))
				Destroy(target);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
