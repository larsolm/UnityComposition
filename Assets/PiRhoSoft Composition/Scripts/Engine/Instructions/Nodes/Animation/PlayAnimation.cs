using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Animation")]
	[HelpURL(Composition.DocumentationUrl + "play-animation")]
	public class PlayAnimation : InstructionGraphNode, IIsImmediate
	{
		private const string _animationNotFoundWarning = "(CAPANF) Unable to play animation on {0}: animation could not be found";
		private const string _noAnimatorWarning = "(CAPANA) Unable to find animator {0}: the animator could not be found";
		private const string _animationNullWarning = "(CAPAAN) Unable to play animation on {0}: animation must not be null";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether the animation should be a variable reference or an actual animation clip")]
		[EnumButtons]
		public VariableSourceType Type;

		[Tooltip("The animation player to play the animation on")]
		public VariableReference Target = new VariableReference();

		[Tooltip("The animation to play")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Value)]
		public AnimationClip Animation;

		[Tooltip("The reference to the animation to play")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Reference)]
		public VariableReference AnimationReference = new VariableReference();

		[Tooltip("Whether to wait for the animation to finish")]
		public bool WaitForCompletion = false;

		public bool IsExecutionImmediate => !WaitForCompletion;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<AnimationPlayer>(Target.RootName));

			if (Type == VariableSourceType.Reference && InstructionStore.IsInput(AnimationReference))
				inputs.Add(VariableDefinition.Create<AnimationClip>(AnimationReference.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Target.GetValue(variables).TryGetObject(out AnimationPlayer player))
			{
				if (Type == VariableSourceType.Value)
				{
					yield return Play(player, Animation);
				}
				else if (Type == VariableSourceType.Reference)
				{
					if (AnimationReference.GetValue(variables).TryGetObject(out AnimationClip animation))
						yield return Play(player, animation);
					else
						Debug.LogWarningFormat(this, _animationNotFoundWarning, player);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _noAnimatorWarning, Target);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator Play(AnimationPlayer player, AnimationClip animation)
		{
			if (animation)
			{
				if (WaitForCompletion)
					yield return player.PlayAnimationAndWait(animation);
				else
					player.PlayAnimation(animation);
			}
			else
			{
				Debug.LogWarningFormat(this, _animationNullWarning, player);
			}
		}
	}
}
