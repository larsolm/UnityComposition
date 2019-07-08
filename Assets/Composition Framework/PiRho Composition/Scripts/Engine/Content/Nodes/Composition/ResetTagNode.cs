using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Composition/Reset Tag", 31)]
	[HelpURL(Composition.DocumentationUrl + "reset-tag-node")]
	public class ResetTagNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The object containing the variables to reset")]
		public VariableReference Object;

		[Tooltip("The tag to reset")]
		public string Tag;

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveInterface(variables, Object, out IVariableReset reset))
				reset.ResetTag(Tag);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
