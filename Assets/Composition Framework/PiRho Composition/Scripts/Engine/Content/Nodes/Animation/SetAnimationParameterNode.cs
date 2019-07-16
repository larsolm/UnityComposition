using PiRhoSoft.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Animation/Set Animation Parameter", 2)]
	[HelpURL(Composition.DocumentationUrl + "set-animation-parameter-node")]
	public class SetAnimationParameterNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animator to set the parameter on")]
		[VariableConstraint(typeof(Animator))]
		public VariableReference Animator;

		[Tooltip("The name of the parameter to set")]
		[Inline]
		public StringVariableSource Parameter = new StringVariableSource();

		[Tooltip("The type of the parameter to set")]
		[EnumButtons]
		public AnimatorControllerParameterType Type;

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Bool)]
		[Inline]
		public BoolVariableSource BoolValue = new BoolVariableSource();

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Int)]
		[Inline]
		public IntVariableSource IntValue = new IntVariableSource();

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Float)]
		[Inline]
		public FloatVariableSource FloatValue = new FloatVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (GraphStore.IsInput(Animator))
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

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
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
