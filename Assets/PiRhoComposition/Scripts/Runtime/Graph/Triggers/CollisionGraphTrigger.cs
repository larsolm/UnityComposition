using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "collision-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Collision Graph Trigger")]
	public class CollisionGraphTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is entered")]
		public GraphCaller EnterGraph = new GraphCaller();

		[Tooltip("The graph to run when this object is exited")]
		public GraphCaller ExitGraph = new GraphCaller();

		private VariableAccess _variables;

		void Awake()
		{
			_variables = new VariableAccess(gameObject);
		}

		void OnCollisionEnter(Collision collision)
		{
			Enter();
		}

		void OnCollisionEnter2D(Collision2D collision)
		{
			Enter();
		}

		void OnTriggerEnter(Collider collider)
		{
			Enter();
		}

		void OnTriggerEnter2D(Collider2D collider)
		{
			Enter();
		}

		void OnCollisionExit(Collision collision)
		{
			Exit();
		}

		void OnCollisionExit2D(Collision2D collision)
		{
			Exit();
		}

		void OnTriggerExit(Collider collider)
		{
			Exit();
		}

		void OnTriggerExit2D(Collider2D collider)
		{
			Exit();
		}

		public void Enter()
		{
			Run(EnterGraph);
		}

		public void Exit()
		{
			Run(ExitGraph);
		}

		private void Run(GraphCaller graph)
		{
			if (graph.Graph && !graph.IsRunning)
				CompositionManager.Instance.RunGraph(graph, _variables, _variables.This);
		}
	}
}
