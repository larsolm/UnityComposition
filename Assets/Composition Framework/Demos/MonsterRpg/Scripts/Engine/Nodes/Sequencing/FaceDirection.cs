using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Sequencing/Face Direction", 10)]
	public class FaceDirection : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to make face")]
		[VariableReference(typeof(Mover))]
		public VariableLookupReference Mover = new VariableLookupReference();

		[Tooltip("The direction to face")]
		[EnumButtons]
		public MovementDirection Direction;

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Mover, out Mover mover))
				mover.FaceDirection(Direction);
			
			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
