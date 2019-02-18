using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Set Animation Parameter")]
	[HelpURL(Composition.DocumentationUrl + "set-animation-parameter")]
	public class SetAnimationParameter : InstructionGraphNode
	{
		private const string _animatorNotFoundWarning = "(WASAPANF) Unable to find animator {0}: the animator could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The animator to set the parameter on")]
		public VariableReference Target = new VariableReference();

		[Tooltip("The name of the parameter to set")]
		public string Parameter;

		[Tooltip("The type of the parameter to set")]
		[EnumButtons]
		public AnimatorControllerParameterType Type;

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Bool)]
		public bool BoolValue;

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Int)]
		public int IntValue;

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Float)]
		public float FloatValue;

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<Animator>(Target.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Target.GetValue(variables).TryGetObject(out Animator target))
				Trigger(variables, target);
			else
				Debug.LogWarningFormat(this, _animatorNotFoundWarning, Target);

			graph.GoTo(Next);

			yield break;
		}

		private void Trigger(IVariableStore variables, Animator animator)
		{
			if (!string.IsNullOrEmpty(Parameter))
			{
				switch (Type)
				{
					case AnimatorControllerParameterType.Trigger: animator.SetTrigger(Parameter); break;
					case AnimatorControllerParameterType.Bool: animator.SetBool(Parameter, BoolValue); break;
					case AnimatorControllerParameterType.Int: animator.SetInteger(Parameter, IntValue); break;
					case AnimatorControllerParameterType.Float: animator.SetFloat(Parameter, FloatValue); break;
				}
			}
		}
	}
}
