using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class AnimatedBinding : VariableBinding
	{
		private const string _invalidInputsWarning = "(PCANIA) failed to animate in binding '{0}': the types '{1}' and '{2}' cannot be animated";
		private const string _unassignedOutputWarning = "(PCANIA) failed to animate in binding '{0}': the variable to animate has not been set";
		private const string _missingOutputWarning = "(PCANIA) failed to animate in binding '{0}': the variable '{1}' cannot be found";
		private const string _readOnlyOutputWarning = "(PCANIA) failed to animate in binding '{0}': the variable '{1}' is read only";
		private const string _invalidOutputWarning = "(PCANIA) failed to animate in binding '{0}': the variable '{1}' could not be set";

		[Tooltip("The value the animation starts at (empty to start at the current value)")]
		public VariableReference From = new VariableReference();

		[Tooltip("The value to animate to")]
		public VariableReference To = new VariableReference();

		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			StopAllCoroutines();
			StartCoroutine(Animate(variables, status));
		}

		private IEnumerator Animate(IVariableCollection variables, BindingAnimationStatus status)
		{
			status.Increment();

			var from = From.GetValue(variables);
			var to = To.GetValue(variables);

			yield return Animation.Animate(variables, from, to);

			switch (Animation.Result)
			{
				case AnimatedVariableResult.InvalidInputs: Debug.LogWarningFormat(this, _invalidInputsWarning, name, from.Type, to.Type); break;
				case AnimatedVariableResult.UnassignedOutput: Debug.LogWarningFormat(this, _unassignedOutputWarning, name, from.Type, to.Type); break;
				case AnimatedVariableResult.MissingOutput: Debug.LogWarningFormat(this, _missingOutputWarning, name, Animation.Target); break;
				case AnimatedVariableResult.ReadOnlyOutput: Debug.LogWarningFormat(this, _readOnlyOutputWarning, name, Animation.Target); break;
				case AnimatedVariableResult.InvalidOutput: Debug.LogWarningFormat(this, _invalidOutputWarning, name, Animation.Target); break;
			}

			status.Decrement();
		}
	}
}