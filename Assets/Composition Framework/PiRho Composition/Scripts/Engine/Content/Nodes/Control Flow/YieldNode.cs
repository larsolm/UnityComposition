using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Yield", 23)]
	[HelpURL(Configuration.DocumentationUrl + "yield-node")]
	public class YieldNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			yield return null;
			graph.GoTo(Next, nameof(Next));
		}
	}
}
