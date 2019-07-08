using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Animation/Play Animation State", 1)]
	[HelpURL(Composition.DocumentationUrl + "play-animation-state-node")]
	public class PlayAnimationStateNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animator to set State on")]
		[VariableConstraint(typeof(Animator))]
		public VariableReference Animator;

		[Tooltip("The name of the animation state to play")]
		[Inline]
		public StringVariableSource State = new StringVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject<Animator>(variables, Animator, out var animator))
			{
				if (Resolve(variables, State, out var state))
					animator.Play(state);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
