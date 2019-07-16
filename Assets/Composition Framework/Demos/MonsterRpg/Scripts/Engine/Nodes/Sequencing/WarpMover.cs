using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Sequencing/Warp Mover", 20)]
	public class WarpMover : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to warp")]
		[VariableConstraint(typeof(Mover))]
		public VariableReference Mover = new VariableReference();

		[Tooltip("The position to warp the mover to")]
		public Int2VariableSource Position = new Int2VariableSource();

		[Tooltip("The direction for the mover to face")]
		[EnumButtons]
		public MovementDirection Direction;

		[Tooltip("The movement layer to put the mover on (None or All mean no change)")]
		public CollisionLayer Layer = CollisionLayer.One;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Mover, out Mover mover))
			{
				Resolve(variables, Position, out var position);
				mover.WarpToPosition(position, Direction, Layer);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
