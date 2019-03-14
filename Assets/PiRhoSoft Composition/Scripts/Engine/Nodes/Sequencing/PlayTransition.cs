using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Play Transition", 0)]
	[HelpURL(Composition.DocumentationUrl + "play-transition")]
	public class PlayTransition : InstructionGraphNode
	{
		private const string _transitionMissingWarning = "(WPTTM) Unable to play transition for {0}: the transition could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The transition to play")]
		[InlineDisplay(PropagateLabel = true)]
		public TransitionVariableSource Transition = new TransitionVariableSource();

		[Tooltip("The phase of the transition to run")]
		[EnumButtons]
		public TransitionPhase Phase = TransitionPhase.Out;

		[Tooltip("Whether the Transition should automatically end after its specified duration or remain running")]
		public bool AutoFinish = true;

		[Tooltip("Whether to wait for the Transition to end before moving on to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor
		{
			get
			{
				switch (Phase)
				{
					case TransitionPhase.Out: return Colors.SequencingDark;
					case TransitionPhase.Obscure: return Colors.Sequencing;
					case TransitionPhase.In: return Colors.SequencingLight;
					default: return base.NodeColor;
				}
			}
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Transition.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Transition.TryGetValue(variables, this, out var transition))
			{
				if (WaitForCompletion)
					yield return AutoFinish ? TransitionManager.Instance.RunTransition(transition, Phase) : TransitionManager.Instance.StartTransition(transition, Phase);
				else
					CompositionManager.Instance.StartCoroutine(AutoFinish ? TransitionManager.Instance.RunTransition(transition, Phase) : TransitionManager.Instance.StartTransition(transition, Phase));
			}
			else
			{
				Debug.LogWarningFormat(this, _transitionMissingWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
