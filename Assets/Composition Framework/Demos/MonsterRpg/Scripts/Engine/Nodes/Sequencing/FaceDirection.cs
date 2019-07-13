using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateGraphNodeMenu("Sequencing/Face Direction", 10)]
	public class FaceDirection : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to make face")]
		[VariableConstraint(typeof(Mover))]
		public VariableReference Mover = new VariableReference();

		[Tooltip("The direction to face")]
		[EnumButtons]
		public MovementDirection Direction;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Mover, out Mover mover))
				mover.FaceDirection(Direction);
			
			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
