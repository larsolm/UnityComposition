﻿using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Graph Trigger")]
	public class GraphTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is triggered")]
		public GraphCaller Graph = new GraphCaller();

		public void Run()
		{
			if (Graph.Graph && !Graph.IsRunning)
				CompositionManager.Instance.RunGraph(Graph, CompositionManager.Instance.DefaultStore, VariableValue.Create(gameObject));
		}
	}
}