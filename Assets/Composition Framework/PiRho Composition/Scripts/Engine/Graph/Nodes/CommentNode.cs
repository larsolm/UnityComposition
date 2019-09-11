using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "comment-node")]
	[CreateGraphNodeMenu("Debug/Comment", 400)]
	public class CommentNode : GraphNode
	{
		public override Color NodeColor => new Color(0.13f, 0.24f, 0.14f, 0.8f);

		[Tooltip("The text of the comment")]
		[TextArea(3, 8)]
		public string Comment = "Double click to insert comment";

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			yield break;
		}
	}
}
