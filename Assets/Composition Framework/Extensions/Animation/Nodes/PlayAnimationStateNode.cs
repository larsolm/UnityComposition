using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Animation/Play Animation State", 1)]
	public class PlayAnimationStateNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animator to set State on")]
		[VariableReference(typeof(Animator))]
		public VariableLookupReference Animator = new VariableLookupReference();

		[Tooltip("The name of the animation state to play")]
		[Inline]
		public StringVariableSource State = new StringVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject<Animator>(this, Animator, out var animator))
			{
				if (variables.Resolve(this, State, out var state))
					animator.Play(state);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
