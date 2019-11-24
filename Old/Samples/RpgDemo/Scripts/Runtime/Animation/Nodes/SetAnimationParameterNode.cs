using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Animation/Set Animation Parameter", 2)]
	public class SetAnimationParameterNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animator to set the parameter on")]
		[VariableConstraint(typeof(Animator))]
		public VariableLookupReference Animator = new VariableLookupReference();

		[Tooltip("The name of the parameter to set")]
		public StringVariableSource Parameter = new StringVariableSource();

		[Tooltip("The type of the parameter to set")]
		[EnumButtons]
		public AnimatorControllerParameterType Type;

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Bool)]
		public BoolVariableSource BoolValue = new BoolVariableSource();

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Int)]
		public IntVariableSource IntValue = new IntVariableSource();

		[Tooltip("The value to set the parameter to")]
		[Conditional(nameof(Type), (int)AnimatorControllerParameterType.Float)]
		public FloatVariableSource FloatValue = new FloatVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject(this, Animator, out Animator animator))
				Trigger(variables, animator);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void Trigger(IVariableMap variables, Animator animator)
		{
			if (variables.Resolve(this, Parameter, out var parameter))
			{
				switch (Type)
				{
					case AnimatorControllerParameterType.Bool:
					{
						if (variables.Resolve(this, BoolValue, out var value))
							animator.SetBool(parameter, value);

						break;
					}
					case AnimatorControllerParameterType.Int:
					{
						if (variables.Resolve(this, IntValue, out var value))
							animator.SetInteger(parameter, value);

						break;
					}
					case AnimatorControllerParameterType.Float:
					{
						if (variables.Resolve(this, FloatValue, out var value))
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
