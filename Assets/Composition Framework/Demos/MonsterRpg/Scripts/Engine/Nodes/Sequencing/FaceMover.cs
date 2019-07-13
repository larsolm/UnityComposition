using PiRhoSoft.Composition.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateGraphNodeMenu("Sequencing/Face Mover", 11)]
	public class FaceMover : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to make face")]
		[VariableConstraint(typeof(Mover))]
		public VariableReference Mover = new VariableReference();

		[Tooltip("The mover to face")]
		[VariableConstraint(typeof(Mover))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.Sequencing;
		
		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Mover, out Mover mover) && ResolveObject(variables, Target, out Mover target))
			{
				var dir = target.transform.position - mover.transform.position;
				var direction = Direction.GetDirection(dir);

				target.FaceDirection(direction);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
