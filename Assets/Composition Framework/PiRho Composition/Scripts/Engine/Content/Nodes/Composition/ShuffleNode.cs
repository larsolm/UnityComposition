﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Composition/Shuffle", 20)]
	[HelpURL(Composition.DocumentationUrl + "shuffle-node")]
	public class ShuffleNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The list to shuffle")]
		public VariableReference Variable = new VariableReference();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, Variable, out IVariableList list))
			{
				for (var i = 0; i < list.Count - 2; i++)
				{
					var j = Random.Range(i, list.Count);
					var iValue = list.GetVariable(i);
					var jValue = list.GetVariable(j);

					list.SetVariable(i, jValue);
					list.SetVariable(j, iValue);
				}
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}