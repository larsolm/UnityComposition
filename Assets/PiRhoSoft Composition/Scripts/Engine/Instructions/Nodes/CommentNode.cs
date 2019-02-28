using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "comment-node")]
	public class CommentNode : InstructionGraphNode
	{
		[Tooltip("The text of the comment")]
		[TextArea(3, 8)]
		public string Comment;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield break;
		}
	}
}
