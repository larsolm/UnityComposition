﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Control Flow/Exit", 23)]
	[HelpURL(Composition.DocumentationUrl + "exit-node")]
	public class ExitNode : GraphNode
	{
		public override Color NodeColor => Colors.Break;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			graph.BreakAll();
			yield break;
		}
	}
}