﻿using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Set Animation Parameter", 2)]
	[HelpURL(Composition.DocumentationUrl + "set-animation-parameter-node")]
	public class SetAnimationParameterNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Animator to set the parameter on")]
		[VariableConstraint(typeof(Animator))]
		public VariableReference Animator;

		[Tooltip("The name of the parameter to set")]
		[ClassDisplay(ClassDisplayType.Propogated)]
		public StringVariableSource Parameter = new StringVariableSource();

		[Tooltip("The type of the parameter to set")]
		[EnumDisplay]
		public AnimatorControllerParameterType Type;

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Bool)]
		[ClassDisplay(ClassDisplayType.Propogated)]
		public BoolVariableSource BoolValue = new BoolVariableSource();

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Int)]
		[ClassDisplay(ClassDisplayType.Propogated)]
		public IntVariableSource IntValue = new IntVariableSource();

		[Tooltip("The value to set the parameter to")]
		[ConditionalDisplaySelf(nameof(Type), EnumValue = (int)AnimatorControllerParameterType.Float)]
		[ClassDisplay(ClassDisplayType.Propogated)]
		public FloatVariableSource FloatValue = new FloatVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Animator))
				inputs.Add(new VariableDefinition { Name = Animator.RootName, Definition = ValueDefinition.Create<Animator>() });

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
			if (ResolveObject(variables, Animator, out Animator animator))
				Trigger(variables, animator);

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

						break;
					}
					case AnimatorControllerParameterType.Int:
					{
						if (Resolve(variables, IntValue, out var value))
							animator.SetInteger(parameter, value);

						break;
					}
					case AnimatorControllerParameterType.Float:
					{
						if (Resolve(variables, FloatValue, out var value))
							animator.SetFloat(parameter, value);

						break;
					}
					case AnimatorControllerParameterType.Trigger:
					{
						animator.SetTrigger(parameter);
						break;
					}
				}
			}
		}
	}
}
