using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Trigger")]
	public class GraphTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is triggered")]
		public GraphCaller Graph = new GraphCaller();

		public void Run()
		{
			if (Graph.Graph && !Graph.IsRunning)
				CompositionManager.Instance.RunGraph(Graph, Variable.Object(gameObject));
		}
	}
}
