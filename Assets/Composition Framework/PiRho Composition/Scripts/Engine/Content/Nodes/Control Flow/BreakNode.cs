using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Break", 22)]
	[HelpURL(Configuration.DocumentationUrl + "break-node")]
	public class BreakNode : GraphNode
	{
		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			graph.Break();
			yield break;
		}
	}
}
