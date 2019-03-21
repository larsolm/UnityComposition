using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class AnimationClipVariableSource : VariableSource<AnimationClip> { }

	[CreateInstructionGraphNodeMenu("Animation/Play Animation", 0)]
	[HelpURL(Composition.DocumentationUrl + "play-animation")]
	public class PlayAnimation : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Animation Player to play the clip on")]
		[VariableConstraint(typeof(AnimationPlayer))]
		public VariableReference AnimationPlayer;

		[Tooltip("The Animation Clip to play")]
		[InlineDisplay(PropagateLabel = true)]
		public AnimationClipVariableSource Animation = new AnimationClipVariableSource();

		[Tooltip("Whether to wait for the animation to finish")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject<AnimationPlayer>(variables, AnimationPlayer, out var player))
			{
				if (ResolveObject(variables, Animation, out var animation))
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
