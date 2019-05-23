using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "collision-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Collision Graph Trigger")]
	public class CollisionGraphTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller EnterGraph = new InstructionCaller();

		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller ExitGraph = new InstructionCaller();

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

		private void Run(InstructionCaller graph)
		{
			if (graph.Instruction && !graph.IsRunning)
				CompositionManager.Instance.RunInstruction(graph, CompositionManager.Instance.DefaultStore, VariableValue.Create(gameObject));
		}
	}
}
