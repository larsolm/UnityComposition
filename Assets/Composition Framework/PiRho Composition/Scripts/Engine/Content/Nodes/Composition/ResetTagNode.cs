﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Composition/Reset Tag", 31)]
	[HelpURL(Configuration.DocumentationUrl + "reset-tag-node")]
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
			if (ResolveInterface(variables, Object, out IResettableVariables reset))
				reset.ResetTag(Tag);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
