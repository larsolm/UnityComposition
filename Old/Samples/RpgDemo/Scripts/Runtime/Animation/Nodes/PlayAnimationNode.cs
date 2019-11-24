using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[Serializable]
	public class AnimationClipVariableSource : VariableSource<AnimationClip> { }

	[CreateGraphNodeMenu("Animation/Play Animation", 0)]
	public class PlayAnimationNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animation Player to play the clip on")]
		[VariableConstraint(typeof(AnimationPlayer))]
		public VariableLookupReference AnimationPlayer = new VariableLookupReference();

		[Tooltip("The Animation to play")]
		public AnimationClipVariableSource Animation = new AnimationClipVariableSource();

		[Tooltip("Whether to wait for the animation to finish")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<AnimationPlayer>(this, AnimationPlayer, out var player))
			{
				if (variables.ResolveObject(this, Animation, out var animation))
				{
					if (WaitForCompletion)
						yield return player.PlayAnimationAndWait(animation);
					else
						player.PlayAnimation(animation);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
