using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Thaw Mover", 111)]
	public class ThawMover : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to thaw")]
		[VariableConstraint(typeof(Mover))]
		public VariableLookupReference Mover = new VariableLookupReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<Mover>(this, Mover, out var mover))
				mover.Thaw();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
