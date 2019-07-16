using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "comment-node")]
	[CreateGraphNodeMenu("Debug/Comment", 400)]
	public class CommentNode : GraphNode
	{
		public override Color NodeColor => new Color(0.13f, 0.24f, 0.14f, 0.8f);

		[Tooltip("The text of the comment")]
		[TextArea(3, 8)]
		public string Comment;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			yield break;
		}
	}
}
