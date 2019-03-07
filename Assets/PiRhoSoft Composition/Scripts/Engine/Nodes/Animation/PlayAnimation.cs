using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class AnimationClipVariableSource : ObjectVariableSource<AnimationClip> { }

	[CreateInstructionGraphNodeMenu("Animation/Play Animation")]
	[HelpURL(Composition.DocumentationUrl + "play-animation")]
	public class PlayAnimation : InstructionGraphNode
	{
		private const string _animationNotFoundWarning = "(CAPANF) Unable to play animation for {0}: the animation could not be found";
		private const string _invalidPlayerWarning = "(CAPAIP) Unable to play animation for {0}: the given variables must me an AnimationPlayer";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The animation to play")]
		[InlineDisplay(PropagateLabel = true)]
		public AnimationClipVariableSource Animation = new AnimationClipVariableSource();

		[Tooltip("Whether to wait for the animation to finish")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Animation.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is AnimationPlayer player)
			{
				if (Animation.TryGetValue(variables, this, out var animation))
				{
					if (WaitForCompletion)
						yield return player.PlayAnimationAndWait(animation);
					else
						player.PlayAnimation(animation);
				}
				else
				{
					Debug.LogWarningFormat(this, _animationNotFoundWarning, player);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidPlayerWarning, name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
