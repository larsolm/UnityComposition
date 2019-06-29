using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Play Transition", 0)]
	[HelpURL(Composition.DocumentationUrl + "play-transition-node")]
	public class PlayTransitionNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The transition to play")]
		[Inline]
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

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Transition, out var transition))
			{
				if (WaitForCompletion)
					yield return AutoFinish ? TransitionManager.Instance.RunTransition(transition, Phase) : TransitionManager.Instance.StartTransition(transition, Phase);
				else
					TransitionManager.Instance.StartCoroutine(AutoFinish ? TransitionManager.Instance.RunTransition(transition, Phase) : TransitionManager.Instance.StartTransition(transition, Phase));
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
