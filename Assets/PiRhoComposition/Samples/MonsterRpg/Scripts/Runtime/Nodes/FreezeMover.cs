using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Freeze Mover", 110)]
	public class FreezeMover : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to thaw")]
		[VariableConstraint(typeof(Mover))]
		public VariableLookupReference Mover = new VariableLookupReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<Mover>(this, Mover, out var mover))
				mover.Freeze();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
