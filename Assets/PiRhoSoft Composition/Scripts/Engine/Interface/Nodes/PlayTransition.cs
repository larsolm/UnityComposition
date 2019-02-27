using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Interface/Play Transition", 200)]
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

		public override Color GetNodeColor()
		{
			switch (Phase)
			{
				case TransitionPhase.Out: return new Color(0.0f, 0.25f, 0.0f);
				case TransitionPhase.Obscure: return new Color(0.0f, 0.35f, 0.0f);
				case TransitionPhase.In: return new Color(0.0f, 0.45f, 0.0f);
				default: return base.GetNodeColor();
			}
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Transition.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Transition.TryGetValue(variables, this, out var transition))
				yield return AutoFinish ? TransitionManager.Instance.RunTransition(transition, Phase) : TransitionManager.Instance.StartTransition(transition, Phase);
			else
				Debug.LogWarningFormat(this, _transitionMissingWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
