﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "comment-node")]
	[CreateGraphNodeMenu("Debug/Comment", 400)]
	public class CommentNode : GraphNode
	{
		[Tooltip("The text of the comment")]
		[TextArea(3, 8)]
		public string Comment;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			yield break;
		}
	}
}