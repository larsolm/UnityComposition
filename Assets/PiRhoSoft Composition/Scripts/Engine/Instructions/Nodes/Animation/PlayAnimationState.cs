using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Animation State")]
	[HelpURL(Composition.DocumentationUrl + "play-animation-state")]
	public class PlayAnimationState : InstructionGraphNode
	{
		private const string _stateNotFoundWarning = "(CAPASSNF) Unable to play animation state {0}: the state could not be found";
		private const string _animatorNotFoundWarning = "(CAPASANF) Unable to find animator {0}: the animator could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether the animation should be a variable reference or an actual animation clip")]
		[EnumButtons]
		public VariableSourceType Type;

		[Tooltip("The animator to set the controller on")]
		public VariableReference Target = new VariableReference();

		[Tooltip("The animation state to play")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Value)]
		public string State;

		[Tooltip("The reference to the animation state to play")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)VariableSourceType.Reference)]
		public VariableReference StateReference = new VariableReference();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<Animator>(Target.RootName));

			if (Type == VariableSourceType.Reference && InstructionStore.IsInput(StateReference))
				inputs.Add(VariableDefinition.Create(StateReference.RootName, VariableType.String));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Target.GetValue(variables).TryGetObject(out Animator animator))
			{
				if (Type == VariableSourceType.Value)
				{
					animator.Play(State);
				}
				else if (Type == VariableSourceType.Reference)
				{
					if (StateReference.GetValue(variables).TryGetString(out var state))
						animator.Play(state);
					else
						Debug.LogWarningFormat(this, _stateNotFoundWarning, state);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _animatorNotFoundWarning, Target);
			}

			graph.GoTo(Next);

			yield break;
		}
	}
}
