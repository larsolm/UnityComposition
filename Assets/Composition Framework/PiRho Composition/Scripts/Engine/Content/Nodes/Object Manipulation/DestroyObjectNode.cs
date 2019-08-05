﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Destroy Object", 2)]
	[HelpURL(Configuration.DocumentationUrl + "destroy-object-node")]
	public class DestroyObjectNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The target Object to destroy")]
		[VariableReference(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (ResolveObject(variables, Target, out Object target))
				Destroy(target);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
