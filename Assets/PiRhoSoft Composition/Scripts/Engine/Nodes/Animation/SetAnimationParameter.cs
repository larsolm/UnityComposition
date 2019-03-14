using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Set Animation Parameter", 2)]
	[HelpURL(Composition.DocumentationUrl + "set-animation-parameter")]
	public class SetAnimationParameter : InstructionGraphNode
	{
		private const string _animatorNotFoundWarning = "(WASAPANF) Unable to set animation parameter for {0}: the given variables must be an Animator";
		private const string _parameterNotFoundWarning = "(WASAPPNF) Unable to set animation parameter for {0}: the parameter could not be found";
		private const string _valueNotFoundWarning = "(WASAPVNF) Unable to set animation parameter for {0}: the value could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The name of the parameter to set")]
		public StringVariableSource Parameter = new StringVariableSource();

		[Tooltip("The type of the parameter to set")]
		[EnumButtons]
		public AnimatorControllerParameterType Type;

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Bool)]
		[InlineDisplay(PropagateLabel = true)]
		public BooleanVariableSource BoolValue = new BooleanVariableSource();

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Int)]
		[InlineDisplay(PropagateLabel = true)]
		public IntegerVariableSource IntValue = new IntegerVariableSource();

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Float)]
		[InlineDisplay(PropagateLabel = true)]
		public NumberVariableSource FloatValue = new NumberVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Parameter.GetInputs(inputs);

			switch (Type)
			{
				case AnimatorControllerParameterType.Float: FloatValue.GetInputs(inputs); break;
				case AnimatorControllerParameterType.Int: IntValue.GetInputs(inputs); break;
				case AnimatorControllerParameterType.Bool: BoolValue.GetInputs(inputs); break;
				case AnimatorControllerParameterType.Trigger: break;
			}
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is Animator animator)
				Trigger(variables, animator);
			else
				Debug.LogWarningFormat(this, _animatorNotFoundWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void Trigger(IVariableStore variables, Animator animator)
		{
			if (Resolve(variables, Parameter, out var parameter))
			{
				switch (Type)
				{
					case AnimatorControllerParameterType.Bool:
					{
						if (Resolve(variables, BoolValue, out var value))
							animator.SetBool(parameter, value);
						else
							Debug.LogWarningFormat(this, _valueNotFoundWarning, Name);

						break;
					}
					case AnimatorControllerParameterType.Int:
					{
						if (Resolve(variables, IntValue, out var value))
							animator.SetInteger(parameter, value);
						else
							Debug.LogWarningFormat(this, _valueNotFoundWarning, Name);

						break;
					}
					case AnimatorControllerParameterType.Float:
					{
						if (Resolve(variables, FloatValue, out var value))
							animator.SetFloat(parameter, value);
						else
							Debug.LogWarningFormat(this, _valueNotFoundWarning, Name);

						break;
					}
					case AnimatorControllerParameterType.Trigger:
					{
						animator.SetTrigger(parameter);
						break;
					}
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _parameterNotFoundWarning, Name);
			}
		}
	}
}
