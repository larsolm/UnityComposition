using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Animation State", 1)]
	[HelpURL(Composition.DocumentationUrl + "play-animation-state")]
	public class PlayAnimationState : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Animator to set State on")]
		[VariableConstraint(typeof(Animator))]
		public VariableReference Animator;

		[Tooltip("The name of the animation state to play")]
		[ClassDisplay(Type = ClassDisplayType.Propogated)]
		public StringVariableSource State = new StringVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
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
