﻿using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public interface ICollisionTrigger
	{
		void Enter();
		void Exit();
	}

	[HelpURL(Composition.DocumentationUrl + "collision-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Collision Graph Trigger")]
	public class CollisionGraphTrigger : MonoBehaviour, ICollisionTrigger
	{
		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller EnterGraph = new InstructionCaller();

		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller ExitGraph = new InstructionCaller();

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
			if (graph.Instruction)
				CompositionManager.Instance.RunInstruction(graph, VariableValue.Create(this));
		}
	}
}